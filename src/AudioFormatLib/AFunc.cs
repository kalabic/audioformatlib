/******************************************************************************
 *
 * This file is part of AudioFormatLib, which is free software:
 * you can redistribute it and/or modify it under the terms of the
 * GNU Lesser General Public License (LGPL), version 2.1 only,
 * as published by the Free Software Foundation.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * version 2.1 along with this file. If not, see:
 * https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html 
 *
 *****************************************************************************/

using AudioFormatLib.Utils;

namespace AudioFormatLib;


public unsafe delegate ResultT AFunc<ParamsT, ResultT>(
    AudioSpan   input,
    AudioSpan   output,
    ParamsT     param
);

public unsafe delegate void AConversion_Func(
    in ConverterParams CP,
    in AudioSpan input,
    in AudioSpan output
);
