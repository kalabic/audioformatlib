using AudioFormatLib.Buffers;
using AudioFormatLib.IO;
using System.Runtime.CompilerServices;

namespace AudioFormatLib.Extensions;


/// <summary>
/// 
/// Extensions here allow for mixing different input and output types, Arrays and unsafe memory pointers.
/// 
/// </summary>
public unsafe abstract class AudioFunction<ParamsT, ResultT>
    : DisposableBuffer
{
    private readonly BufferCoupler _coupler;

    protected AudioFunction(int numChannels, int inSampleSize, int outSampleSize)
    {
        _coupler = new BufferCoupler(numChannels, inSampleSize, outSampleSize);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _coupler.Dispose();
        }
        base.Dispose(disposing);
    }

    protected IInputProducer<float> GetProducer(int index)
    {
        return _coupler.GetProducer(index);
    }

    protected IOutputConsumer<float> GetConsumer(int index)
    {
        return _coupler.GetConsumer(index);
    }

    protected abstract ResultT Process(ParamsT Param);

    protected abstract ResultT IOErrorOccurred();

    protected abstract ResultT ExceptionOccurred(Exception ex);



    //
    // Process IPtr input/output. All other Ptr_t, Array, APtr & AObjPtr combinations finally invoke this method.
    //

    public unsafe ResultT Process<IN, OUT>(in IPtr<IN> input, in IPtr<OUT> output, ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        if (_coupler.AttachBuffers(input.OffsetPtr, input.Length, output.OffsetPtr, output.Length))
        {
            try
            {
                return Process(Params);
            }
            catch (Exception ex)
            {
                return ExceptionOccurred(ex);
            }
            finally
            {
                _coupler.ReleaseBuffers();
            }
        }
        else
        {
            return IOErrorOccurred();
        }
    }



    //
    // Process Ptr_t & Array combinations of inputs and outputs.
    //

    /// <summary>
    /// 
    /// <para>Input: raw pointer.</para>
    /// <para>Output: raw pointer.</para>
    /// 
    /// </summary>
    /// <typeparam name="IN"></typeparam>
    /// <typeparam name="OUT"></typeparam>
    /// <param name="input"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="output"></param>
    /// <param name="outOffset"></param>
    /// <param name="outLength"></param>
    /// <param name="Params"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(Ptr_t<IN> input, long offset, long length,
                                           Ptr_t<OUT> output, long outOffset, long outLength,
                                           ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        return Process<IN, OUT>((input, offset, length), (output, outOffset, outLength), Params);
    }

    /// <summary>
    /// 
    /// <para>Input: array.</para>
    /// <para>Output: array.</para>
    /// 
    /// </summary>
    /// <typeparam name="IN"></typeparam>
    /// <typeparam name="OUT"></typeparam>
    /// <param name="input"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="output"></param>
    /// <param name="outOffset"></param>
    /// <param name="outLength"></param>
    /// <param name="Params"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(IN[] input, long offset, long length,
                                           OUT[] output, long outOffset, long outLength,
                                           ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        var pinIn = (AObjPtr<IN>)(input, offset, length);
        var pinOut = (AObjPtr<OUT>)(output, outOffset, outLength);
        var result =  Process(ref pinIn, ref pinOut, Params);
        pinIn.Unpin();
        pinOut.Unpin();
        return result;
    }

    /// <summary>
    /// 
    /// <para>Input: raw pointer.</para>
    /// <para>Output: array.</para>
    /// 
    /// </summary>
    /// <typeparam name="IN"></typeparam>
    /// <typeparam name="OUT"></typeparam>
    /// <param name="input"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="output"></param>
    /// <param name="outOffset"></param>
    /// <param name="outLength"></param>
    /// <param name="Params"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(Ptr_t<IN> input, long offset, long length,
                                           OUT[] output, long outOffset, long outLength,
                                           ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        var pinOut = (AObjPtr<OUT>)(output, outOffset, outLength);
        var result = Process<IN, OUT>((input, offset, length), ref pinOut, Params);
        pinOut.Unpin();
        return result;
    }

    /// <summary>
    /// 
    /// <para>Input: array.</para>
    /// <para>Output: raw pointer.</para>
    /// 
    /// </summary>
    /// <typeparam name="IN"></typeparam>
    /// <typeparam name="OUT"></typeparam>
    /// <param name="input"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="output"></param>
    /// <param name="outOffset"></param>
    /// <param name="outLength"></param>
    /// <param name="Params"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(IN[] input, long offset, long length,
                                           Ptr_t<OUT> output, long outOffset, long outLength,
                                           ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        var pinIn = (AObjPtr<IN>)(input, offset, length);
        var result = Process<IN, OUT>(ref pinIn, (output, outOffset, outLength), Params);
        pinIn.Unpin();
        return result;
    }



    //
    // Process APtr & AObjPtr combinations of inputs and outputs.
    //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(in APtr<IN> input, in APtr<OUT> output, ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        return Process((IPtr<IN>)input, (IPtr<OUT>)output, Params);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(ref AObjPtr<IN> input, ref AObjPtr<OUT> output, ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        return Process((IPtr<IN>)input, (IPtr<OUT>)output, Params);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(in APtr<IN> input, ref AObjPtr<OUT> output, ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        return Process((IPtr<IN>)input, (IPtr<OUT>)output, Params);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ResultT Process<IN, OUT>(ref AObjPtr<IN> input, in APtr<OUT> output, ParamsT Params)
         where IN : unmanaged
         where OUT : unmanaged
    {
        return Process((IPtr<IN>)input, (IPtr<OUT>)output, Params);
    }
}
