### About

This is not the highest-quality resampling algorithm available, nor is it the fastest. But it is pretty good and it is quite usable in the context of pure C#/DotNet applications. According to Dominic Mazzoni the best resampling library is __libsamplerate__ by Erik de Castro Lopo.

Some small progress has been made since it was forked from Xianyi Cui's __libresamplesharp__: cache for calculated coefficients was added; input and output interface here implements on-the-fly conversion, something that was made possible in previous versions, but not implemented or not implemented fully.

### Breaking down licensing question

A bit of history is required here to explain why a decision was made to change the license of __AudioFormatLib__ to LGPL.

Source code for resampler in AudioFormatLib is a fork of Xianyi Cui's [libresamplesharp](https://github.com/xycui/libresamplesharp) and he used the source code from [libresample4j](https://github.com/dnault/libresample4j), a Java library by David Nault. David's library is licensed with LGPL Version 2.1.

Furthermore, David made __libresample4j__ as a port of Dominic Mazzoni's [libresample](https://github.com/minorninth/libresample) v0.1.3, which is in turn based on [Julius Smith's](https://ccrma.stanford.edu/~jos/) __Resample v1.7__ library (available for download [here](https://ccrma.stanford.edu/~jos/resample/Free_Resampling_Software.html)). Dominic Mazzoni's __libresample__ is available for use under the terms of either the BSD License or the LGPL License. FYI, Dominic is the one who rewrote it to use single-precision floating-point arithmetic and more:
> ... Resample-1.7 is a fixed-point resampler, and as a result has only limited precision. I rewrote it to use single-precision floating-point arithmetic instead and increased the number of filter coefficients between time steps significantly.


Now back to licensing, Julius Smith's __Resample__ library is released under the GNU Lesser General Public License (LGPL). So <ins>it appears that the LGPL sticks to the whole derivative chain</ins> because the base code is LGPL-only. The BSD option in __libresample__ v0.1.3 cannot override the original LGPL status of __Resample__ v1.7, and a derivative library __libresample4j__ (David's library) is in compliance with that.

### Why LGPL is a Good Fit for AudioFormatLib

It can stay compliant - because LGPL lets it legally and ethically incorporate other LGPL code, now and in the future.
Any accidental license violations from the mixed heritage of older DSP/resampling libraries are avoided this way.

Maintains Wide Usability - LGPL is library-friendly: developers using it in closed-source/proprietary apps can still link to it without needing to open-source their own code, as long as they comply with LGPL requirements (e.g., allow dynamic linking or relinking).

Future-Proof - don’t have to reevaluate license compatibility every time new LGPL code is added.

### About a NuGet package option of using AudioFormatLib under the LGPL

Applications that merely use (i.e., link to) the binary via NuGet are not themselves bound by the LGPL, as long as:
- Application does not statically link the LGPL code into itself.
- Application allows the LGPL library to be replaced or relinked by the end user.
- The source code of library is available on GitHub, and application should provide a link to it.
This way it is safe to publish an LGPL NuGet package, and users can consume it in any kind of app, proprietary or open.

### P.S. - Even more history from the changelog inside man page for Resample on Linux

> The first version of this software was written by Julius O. Smith III at [CCRMA](http://ccrma.stanford.edu) in 1981.  It was called SRCONV and was written in SAIL for PDP-10 compatible machines. The algorithm was first published in Smith, Julius O. and Phil Gossett *["A Flexible Sampling-Rate Conversion Method"](https://ieeexplore.ieee.org/document/1172555), [IEEE Conference on Acoustics, Speech, and Signal Processing, San Diego, March 1984](https://ieeexplore.ieee.org/xpl/conhome/8372/proceeding))*.
>
> Circa 1988, the SRCONV program was translated from SAIL to C by Christopher Lee Fraley working with Roger Dannenberg at CMU.
>
> Since then, the C version has been maintained by Julius O. Smith III.
>
>Sndlib support was added 6/99 by John Gibson.

### Links

A list of (some) audio technology resources for developers:
- Julius Smith’s *"Free Resampling Software"*: [https://ccrma.stanford.edu/~jos/resample/Free_Resampling_Software.html](https://ccrma.stanford.edu/~jos/resample/Free_Resampling_Software.html)
- Julius Smith’s *"Digital Audio Resampling Home Page"*: [https://ccrma.stanford.edu/~jos/resample/](https://ccrma.stanford.edu/~jos/resample/)
- Julius Smith’s *"Music 423 2023 GitLab Project: Research Paper Collection"*: [https://cm-gitlab.stanford.edu/jos/music423-2023](https://cm-gitlab.stanford.edu/jos/music423-2023)
- Page comparing various sampling-rate converters: [https://src.infinitewave.ca/](https://src.infinitewave.ca/)
- *"Python for Scientific Audio"*, a comprehensive, curated list of python software/tools related and used for scientific research in audio/music applications: [https://github.com/faroit/awesome-python-scientific-audio](https://github.com/faroit/awesome-python-scientific-audio)
- *"Awesome-Audio"*, a curated list of awesome audio technology resources for developers: [https://github.com/DolbyIO/awesome-audio](https://github.com/DolbyIO/awesome-audio)
- *"Awesome Audio DSP"*, audio DSP (digital signal processing) and plugin development resources: [https://github.com/BillyDM/awesome-audio-dsp](https://github.com/BillyDM/awesome-audio-dsp)
