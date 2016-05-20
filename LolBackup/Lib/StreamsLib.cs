//////////////////////////////////////////////////////////////////////////////////////
// Author				: Shukri Adams												//
// Contact				: shukri.adams@gmail.com									//
//																					//
// vcFramework : A reuseable library of utility classes                             //
// Copyright (C)																	//
//////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;

namespace vcFramework.IO.Streams
{
	/// <summary>  Static class for working with basic streams </summary>
	public class StreamsLib
	{

	    /// <summary> 
        /// Takes a string and converts to an binary stream 
        /// </summary>
	    /// <param name="input"></param>
	    /// <param name="blockSize"></param>
	    /// <returns></returns>
	    public static Stream StringToBinaryStream(string input,int blockSize)
		{
	        char[] charBuffer = new char[blockSize];
            byte[] byteBuffer = new byte[blockSize];
			StringReader reader	= null;
	        Encoder encoder = Encoding.Default.GetEncoder(); 
			
			try
			{

                Stream mem = new MemoryStream();
				// puts string into string reader stream
                reader = new StringReader(input);
			
				// stores total length of string
                long totalLength = input.Length;

                while (totalLength > 0)
				{
					// sets normal transfer block size
                    int bufferSize = blockSize;							// actual used block size - is usually = intBlockSize, but can be less on final block
					// if this is the the final block transfer, transfer size is the
					// remainder of the total original size divided by intBlockSize
                    if (totalLength < blockSize)
					{
                        bufferSize = Convert.ToInt32(totalLength);
						// recreate arrays to accurately fit input
                        charBuffer = new char[bufferSize];
                        byteBuffer = new byte[bufferSize];
					}
					


					// writes block of string to char array
                    reader.Read(charBuffer, 0, bufferSize);
				
					// converts char block to binary
                    encoder.GetBytes(charBuffer, 0, bufferSize, byteBuffer, 0, true);

					// writes binary array to file
                    mem.Write(byteBuffer, 0, bufferSize);

                    totalLength = totalLength - blockSize;
				}
				

				// resets stream start position
                mem.Seek(0, SeekOrigin.Begin);

                return mem;
				
			}
			finally
			{
				// clean up 
                if (reader != null)
                {
                    reader.Close();
                }
			}
		}

		

		/// <summary> 
        /// Returns contents of binary stream as string
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
		/// <returns></returns>
		public static string BinaryStreamToString(Stream stream,int blockSize)
		{
		    char[] charBuffer = new char[blockSize];
            byte[] byteBuffer = new byte[blockSize];
			Decoder decoder = Encoding.Default.GetDecoder(); 
			StringBuilder s = new StringBuilder();
			
			// rewinds stream
            stream.Seek(0, SeekOrigin.Begin);

			// stores total length of stream
            long totalLength = stream.Length;

            while (totalLength > 0)
			{
				// sets normal transfer block size
                int bufferSize = blockSize;
				
				// if this is the the final block transfer, transfer size is the
				// remainder of the total original size divided by intBlockSize
                if (totalLength < blockSize)
				{
                    bufferSize = Convert.ToInt32(totalLength);
					// recreate arrays to accurately fit input
                    charBuffer = new char[bufferSize];
                    byteBuffer = new byte[bufferSize];
				}
			
				// writes block of to byte array
                stream.Read(byteBuffer, 0, bufferSize);
				
				// converts byte block to char
                decoder.GetChars(byteBuffer, 0, bufferSize, charBuffer, 0);

				// writes chars to stringbuilder
                s.Append(new string(charBuffer));

                totalLength = totalLength - bufferSize;

			}

			return s.ToString();
		}

		

	}
}
