using AudioFormatLib.Buffers;
using AudioFormatLib.Extensions;
using AudioFormatLib.IO;

namespace AudioFormatLib.Tests;


public sealed class Pcm16BufferViewTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NativeOrder_IsResolvedAndReadBehaviorIsExposed(bool waitForCompleteRead)
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(
                ASampleValueFormat.S16,
                8_000,
                1,
                byteOrder: AByteOrder.Native),
            sizeof(short) * 4,
            waitForCompleteRead);

        Assert.Equal(
            BitConverter.IsLittleEndian
                ? AByteOrder.LittleEndian
                : AByteOrder.BigEndian,
            buffer.Format.ByteOrder);
        Assert.Equal(buffer.Format.ByteOrder, buffer.Input.Format.ByteOrder);
        Assert.Equal(buffer.Format.ByteOrder, buffer.Output.Format.ByteOrder);
        Assert.Equal(waitForCompleteRead, buffer.WaitForCompleteRead);
    }

    [Theory]
    [InlineData(ASampleValueFormat.S16, true)]
    [InlineData(ASampleValueFormat.P_S16, true)]
    [InlineData(ASampleValueFormat.U8, false)]
    [InlineData(ASampleValueFormat.S32, false)]
    [InlineData(ASampleValueFormat.FLOAT, false)]
    [InlineData(ASampleValueFormat.DOUBLE, false)]
    [InlineData(ASampleValueFormat.S64, false)]
    public void Pcm16ValueAndFrameViews_AreNullableFormatCapabilities(
        ASampleValueFormat sampleValueFormat,
        bool expected)
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(sampleValueFormat, 8_000, 1),
            sampleValueFormat.Size() * 4);

        Assert.Equal(expected, buffer.Input.Pcm16Values is not null);
        Assert.Equal(expected, buffer.Output.Pcm16Values is not null);
        Assert.Equal(expected, buffer.Input.Pcm16Frames is not null);
        Assert.Equal(expected, buffer.Output.Pcm16Frames is not null);
    }

    [Theory]
    [InlineData(AByteOrder.LittleEndian, 0x34, 0x12, 0xFE, 0xFF)]
    [InlineData(AByteOrder.BigEndian, 0x12, 0x34, 0xFF, 0xFE)]
    public void ValueWrites_ProduceConfiguredRawByteOrder(
        AByteOrder byteOrder,
        byte first,
        byte second,
        byte third,
        byte fourth)
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(8, byteOrder);
        IPcm16ValueInput input = Assert.IsAssignableFrom<IPcm16ValueInput>(
            buffer.Input.Pcm16Values);

        Assert.Equal(2, input.Write(new short[] { 0x1234, -2 }.AsSpan()));

        byte[] bytes = new byte[4];
        Assert.Equal(bytes.Length, buffer.Output.Buffer.Read(bytes, 0, bytes.Length));
        Assert.Equal(new byte[] { first, second, third, fourth }, bytes);
    }

    [Theory]
    [InlineData(AByteOrder.LittleEndian, 0x34, 0x12)]
    [InlineData(AByteOrder.BigEndian, 0x12, 0x34)]
    public void RawBytes_AreDecodedByPcm16ValueView(
        AByteOrder byteOrder,
        byte first,
        byte second)
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(4, byteOrder);
        byte[] bytes = [first, second];
        Assert.Equal(bytes.Length, buffer.Input.Buffer.Write(bytes, 0, bytes.Length));

        Span<short> destination = stackalloc short[1];
        Assert.Equal(1, buffer.Output.Pcm16Values!.Read(destination));
        Assert.Equal((short)0x1234, destination[0]);
    }

    [Fact]
    public unsafe void ArrayPointerAndSpanOperations_Interoperate()
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(16, AByteOrder.BigEndian);
        IPcm16ValueInput input = buffer.Input.Pcm16Values!;
        IPcm16ValueOutput output = buffer.Output.Pcm16Values!;

        short[] source = [99, 1, 2, 3, 99];
        Assert.Equal(3, input.Write(source, 1, 3));

        short[] pointerSource = [4, 5];
        fixed (short* sourcePtr = pointerSource)
        {
            Assert.True(input.TryWrite(sourcePtr, 0, pointerSource.Length));
        }

        Span<short> first = stackalloc short[3];
        Assert.Equal(3, output.Read(first));
        Assert.Equal(new short[] { 1, 2, 3 }, first.ToArray());

        short[] pointerDestination = new short[2];
        fixed (short* destinationPtr = pointerDestination)
        {
            Assert.True(output.TryRead(
                destinationPtr,
                0,
                pointerDestination.Length));
        }

        Assert.Equal(pointerSource, pointerDestination);
    }

    [Fact]
    public void AtomicFailures_DoNotMutateRingOrDestination()
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(4);
        IPcm16ValueInput input = buffer.Input.Pcm16Values!;
        IPcm16ValueOutput output = buffer.Output.Pcm16Values!;

        Assert.False(input.TryWrite(new short[] { 1, 2, 3 }.AsSpan()));
        Assert.Equal(0, buffer.StoredByteCount);

        Assert.True(input.TryWrite(new short[] { 4, 5 }.AsSpan()));
        short[] destination = [9, 9, 9];
        Assert.False(output.TryRead(destination.AsSpan()));
        Assert.Equal(new short[] { 9, 9, 9 }, destination);
        Assert.Equal(2, output.Count);
    }

    [Fact]
    public void FrameOperations_TransferOnlyCompleteInterleavedFrames()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 2),
            sizeof(short) * 2 * 3);
        IPcm16FrameInput input = buffer.Input.Pcm16Frames!;
        IPcm16FrameOutput output = buffer.Output.Pcm16Frames!;
        short[] source = [0, 1, 2, 3, 4, 5, 6, 7];

        Assert.Equal(3, input.Write(source.AsSpan()));
        Assert.Equal(6, buffer.Input.Pcm16Values!.Count);
        Assert.Equal(3, input.Count);
        Assert.Equal(0, input.FreeCapacity);

        short[] destination = Enumerable.Repeat((short)-1, 8).ToArray();
        Assert.Equal(3, output.Read(destination.AsSpan()));
        Assert.Equal(source[..6], destination[..6]);
        Assert.Equal(new short[] { -1, -1 }, destination[6..]);
    }

    [Fact]
    public void ValueAndFrameMeasurements_UseTheirViewUnits()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 2),
            sizeof(short) * 2 * 3);
        IPcm16ValueInput values = buffer.Input.Pcm16Values!;
        IPcm16FrameOutput frames = buffer.Output.Pcm16Frames!;

        Assert.Equal(6, values.Capacity);
        Assert.Equal(0, values.Count);
        Assert.Equal(6, values.FreeCapacity);
        Assert.Equal(3, frames.Capacity);
        Assert.Equal(0, frames.Count);
        Assert.Equal(3, frames.FreeCapacity);

        Assert.True(values.TryWrite(new short[] { 1, 2 }.AsSpan()));

        Assert.Equal(2, values.Count);
        Assert.Equal(4, values.FreeCapacity);
        Assert.Equal(1, frames.Count);
        Assert.Equal(2, frames.FreeCapacity);
    }

    [Fact]
    public void FrameArrayOffsets_AreMeasuredInFrames()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 2),
            sizeof(short) * 2 * 2);
        short[] source = [90, 91, 1, 2, 3, 4, 92, 93];
        short[] destination = Enumerable.Repeat((short)-1, 8).ToArray();

        Assert.True(buffer.Input.Pcm16Frames!.TryWrite(source, 1, 2));
        Assert.True(buffer.Output.Pcm16Frames!.TryRead(destination, 2, 2));
        Assert.Equal(new short[] { -1, -1, -1, -1, 1, 2, 3, 4 }, destination);
    }

    [Fact]
    public unsafe void FramePointerOperations_UseFrameOffsetsAndCounts()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 2),
            sizeof(short) * 2 * 2);
        short[] source = [1, 2, 3, 4];
        short[] destination = new short[4];

        fixed (short* sourcePtr = source)
        fixed (short* destinationPtr = destination)
        {
            Assert.True(buffer.Input.Pcm16Frames!.TryWrite(sourcePtr, 0, 2));
            Assert.True(buffer.Output.Pcm16Frames!.TryRead(destinationPtr, 0, 2));
        }

        Assert.Equal(source, destination);
    }

    [Fact]
    public void InvalidFrameSpan_DoesNotMutateTheRing()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 2),
            sizeof(short) * 4);

        Assert.Throws<ArgumentException>(
            () => buffer.Input.Pcm16Frames!.Write(new short[3].AsSpan()));
        Assert.Equal(0, buffer.StoredByteCount);
    }

    [Fact]
    public void AdvanceOperations_UseTheirDeclaredUnits()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 2),
            sizeof(short) * 2 * 3);
        IPcm16ValueOutput valueOutput = buffer.Output.Pcm16Values!;
        IPcm16FrameOutput frameOutput = buffer.Output.Pcm16Frames!;
        Assert.True(buffer.Input.Pcm16Frames!.TryWrite(
            new short[] { 1, 2, 3, 4, 5, 6 }.AsSpan()));

        frameOutput.Advance(1);
        Assert.Equal(4, valueOutput.Count);
        Assert.Equal(2, frameOutput.Count);

        valueOutput.Advance(1);
        Assert.Equal(3, valueOutput.Count);
        Assert.Equal(1, frameOutput.Count);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => valueOutput.Advance(-1));
        Assert.Throws<OverflowException>(
            () => frameOutput.Advance(int.MaxValue));
        Assert.Equal(3, valueOutput.Count);
    }

    [Fact]
    public void MultichannelPlanarFrameViews_AreUnavailable()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.P_S16, 8_000, 2, planar: true),
            sizeof(short) * 4);

        Assert.NotNull(buffer.Input.Pcm16Values);
        Assert.NotNull(buffer.Output.Pcm16Values);
        Assert.Null(buffer.Input.Pcm16Frames);
        Assert.Null(buffer.Output.Pcm16Frames);
    }

    [Fact]
    public void ExistingShortArrayApi_RemainsEndianCorrect()
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(4, AByteOrder.BigEndian);
        short[] source = [0x1234];
        Assert.Equal(1, buffer.Input.Buffer.Write(source, 0, source.Length));

        byte[] raw = new byte[2];
        Assert.Equal(2, buffer.Output.Buffer.Read(raw, 0, raw.Length));
        Assert.Equal(new byte[] { 0x12, 0x34 }, raw);
    }

    [Fact]
    public void ExistingShortArrayApi_RejectsNonPcm16Buffers()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S32, 8_000, 1),
            sizeof(int) * 2);

        Assert.Throws<InvalidOperationException>(
            () => buffer.Input.Buffer.Write(new short[1], 0, 1));
        Assert.Throws<InvalidOperationException>(
            () => buffer.Output.Buffer.Read(new short[1], 0, 1));
    }

    [Fact]
    public void ExactExtensions_AreAtomic()
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(4);
        short[] source = [1, 2, 3];
        Assert.Throws<InvalidOperationException>(
            () => buffer.WriteSampleValuesExactly(source, 0, source.Length));
        Assert.Equal(0, buffer.StoredByteCount);

        buffer.WriteSampleValuesExactly(source, 0, 2);
        short[] destination = [9, 9, 9];
        Assert.Throws<InvalidOperationException>(
            () => buffer.ReadSampleValuesExactly(
                destination,
                0,
                destination.Length));
        Assert.Equal(new short[] { 9, 9, 9 }, destination);
        Assert.Equal(2, buffer.StoredSampleValueCount);
    }

    [Fact]
    public async Task WaitableRead_WaitsForTheCompleteValueRequest()
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(
            8,
            waitForCompleteRead: true);
        short[] destination = new short[3];
        CancellationToken cancellation = TestContext.Current.CancellationToken;
        Task<int> read = Task.Run(
            () => buffer.Output.Pcm16Values!.Read(destination, 0, 3),
            cancellation);

        Assert.Equal(2, buffer.Input.Pcm16Values!.Write([1, 2], 0, 2));
        await Task.Delay(50, cancellation);
        Assert.False(read.IsCompleted);

        Assert.Equal(1, buffer.Input.Pcm16Values!.Write([3], 0, 1));
        Assert.Equal(3, await read.WaitAsync(cancellation));
        Assert.Equal(new short[] { 1, 2, 3 }, destination);
    }

    [Fact]
    public async Task WaitableFrameRead_WaitsForTheCompleteFrameRequest()
    {
        using AudioStreamBuffer buffer = CreateBuffer(
            new APcmFormat(ASampleValueFormat.S16, 8_000, 2),
            sizeof(short) * 2 * 4,
            waitForCompleteRead: true);
        short[] destination = new short[4];
        CancellationToken cancellation = TestContext.Current.CancellationToken;
        Task<int> read = Task.Run(
            () => buffer.Output.Pcm16Frames!.Read(destination, 0, 2),
            cancellation);

        Assert.Equal(1, buffer.Input.Pcm16Frames!.Write([1, 2], 0, 1));
        await Task.Delay(50, cancellation);
        Assert.False(read.IsCompleted);

        Assert.Equal(1, buffer.Input.Pcm16Frames!.Write([3, 4], 0, 1));
        Assert.Equal(2, await read.WaitAsync(cancellation));
        Assert.Equal(new short[] { 1, 2, 3, 4 }, destination);
    }

    [Fact]
    public async Task ClosingWaitableBuffer_WakesBlockedPcm16ValueRead()
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(
            8,
            waitForCompleteRead: true);
        CancellationToken cancellation = TestContext.Current.CancellationToken;
        Task<int> read = Task.Run(
            () => buffer.Output.Pcm16Values!.Read(new short[2], 0, 2),
            cancellation);

        await Task.Delay(50, cancellation);
        buffer.CloseBuffer();

        Assert.Equal(0, await read.WaitAsync(cancellation));
    }

    [Fact]
    public async Task WaitablePcm16ValueOutput_SupportsMultipleReaders()
    {
        using AudioStreamBuffer buffer = CreatePcm16Buffer(
            8,
            waitForCompleteRead: true);
        CancellationToken cancellation = TestContext.Current.CancellationToken;
        short[] first = new short[1];
        short[] second = new short[1];
        Task<int> firstRead = Task.Run(
            () => buffer.Output.Pcm16Values!.Read(first, 0, 1),
            cancellation);
        Task<int> secondRead = Task.Run(
            () => buffer.Output.Pcm16Values!.Read(second, 0, 1),
            cancellation);

        await Task.Delay(50, cancellation);
        Assert.True(buffer.Input.Pcm16Values!.TryWrite(
            new short[] { 10, 20 }.AsSpan()));

        Assert.Equal(1, await firstRead.WaitAsync(cancellation));
        Assert.Equal(1, await secondRead.WaitAsync(cancellation));
        Assert.Equal(new short[] { 10, 20 }, new[] { first[0], second[0] }.Order());
    }

    private static AudioStreamBuffer CreatePcm16Buffer(
        int byteCapacity,
        AByteOrder byteOrder = AByteOrder.Native,
        bool waitForCompleteRead = false)
    {
        return CreateBuffer(
            new APcmFormat(
                ASampleValueFormat.S16,
                8_000,
                1,
                byteOrder: byteOrder),
            byteCapacity,
            waitForCompleteRead);
    }

    private static AudioStreamBuffer CreateBuffer(
        APcmFormat format,
        int byteCapacity,
        bool waitForCompleteRead = false)
    {
        return new AudioStreamBuffer(new ABufferParams
        {
            Format = format,
            BufferSize = byteCapacity,
            WaitForCompleteRead = waitForCompleteRead,
        });
    }
}
