/*
Copyright (c) 2005, Marc Clifton
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list
  of conditions and the following disclaimer. 

* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or other
  materials provided with the distribution. 
 
* Neither the name of MyXaml nor the names of its contributors may be
  used to endorse or promote products derived from this software without specific
  prior written permission. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using System;
using System.ComponentModel;

namespace ServerLibrary
{

    public class ConverterException : ApplicationException
    {
        public ConverterException(string message)
            : base(message)
        {
        }
    }

    public class Converter
    {
        public static object Convert(object source, Type destinationType)
        {
            object result = source;

            if ((source != null) && (source != DBNull.Value))
            {
                Type sourceType = source.GetType();

                if ((sourceType.FullName == "System.Object") || 
                    (destinationType.FullName == "System.Object"))
                {
                    result = source;
                }
                else
                {
                    if (sourceType != destinationType)
                    {
                        TypeConverter tcSource = TypeDescriptor.GetConverter(sourceType);
                        TypeConverter tcDestination = TypeDescriptor.GetConverter(destinationType);

                        if (tcSource.CanConvertTo(destinationType))
                        {
                            result = tcSource.ConvertTo(source, destinationType);
                        }
                        else if (tcDestination.CanConvertFrom(sourceType))
                        {
                            if (sourceType.FullName == "System.String")
                            {
                                result = tcDestination.ConvertFromInvariantString((string)source);
                            }
                            else
                            {
                                result = tcDestination.ConvertFrom(source);
                            }
                        }
                        else
                        {
                            // If the target type is a base class of the source type, then we don't need to do any conversion.
                            if (destinationType.IsAssignableFrom(sourceType))
                            {
                                result = source;
                            }
                            else
                            {
                                // If no conversion exists, throw an exception.
                                throw new ConverterException("Can't convert from " + source.GetType().FullName + " to " + destinationType.FullName);
                            }
                        }
                    }
                }
            }
            else if (source == DBNull.Value)
            {
                if (destinationType.FullName == "System.String")
                {
                    // convert DBNull.Value to null for strings.
                    result = null;
                }
            }

            return result;
        }
    }
}
