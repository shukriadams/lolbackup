//////////////////////////////////////////////////////////////////////////////////////
// Author				: Shukri Adams												//
// Contact				: shukri.adams@gmail.com									//
//																					//
// vcFramework : A reuseable library of utility classes                             //
// Copyright (C)																	//
//////////////////////////////////////////////////////////////////////////////////////

using System;

namespace vcFramework.Parsers
{
    /// <summary> 
    /// Static class library of various string manipulation methods. Note : contains
    /// some very old code that is slowly being refactored or cleaned out. Yes, there be
    /// Hungarian dragons here.
    /// </summary>
    public class ParserLib
    {

    

        /// <summary> 
        /// Written to replace the standard .IndexOf String method 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static int IndexOfFixed(
            string main,
            string sub
            )
        {
            if (main.IndexOf(sub) == -1)
                return -1;

            if (sub.Length == 1)
                return main.IndexOf(Convert.ToChar(sub));

            return main.IndexOf(sub);
        }



        /// <summary> 
        /// Written to replace the standard .IndexOf String method 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static int IndexOfFixed(
            string main,
            string sub,
            int startPosition
            )
        {
            if (main.IndexOf(sub, startPosition) == -1)
                return -1;

            if (sub.Length == 1)
                return main.IndexOf(Convert.ToChar(sub), startPosition);
            return main.IndexOf(sub, startPosition);
        }


        /// <summary> Finds and replaces text while ignore cases. </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="replaceWith"></param>
        /// <returns></returns>
        public static string ReplaceNoCase(
            string main,
            string sub,
            string replaceWith
            )
        {
            //THIS IS A CASE-UNSPECIFIC WAY OF REPLACING TEXT IN A STRING
            //METHOD IS PRETTY SLOPPY NOW, AND NEEDS TO BE TIGHTENED UP WITH FEWER VARS
            int i;
            int intOccurences;
            int intCurrentPosition;
            int instNextPosition;
            string strStringTemp;
            string strSubStrTemp;
            string strOutput = "";



            if (main.Length == 0)
            {//Exit Function;
            }

            //MAKES TEMPORARY COPIES IN LOWER CASE
            strStringTemp = main.ToLower();
            strSubStrTemp = sub.ToLower();

            //COUNTS HOW MANY TIMES THE SUBSTRING OCCURS IN STRING
            intOccurences = 0;
            intOccurences = StringCount(strStringTemp, strSubStrTemp);

            if (intOccurences != 0)
            {

                //GETS FIRST PART OF STRING, UP UNTIL FIRST OCCURENCE OF SUBSTRING
                strOutput = main.Substring(0, IndexOfFixed(strStringTemp, strSubStrTemp));

                //FIND POSITION IN STRING WHERE FIRST SUBSTR OCCURS
                intCurrentPosition = IndexOfFixed(strStringTemp, strSubStrTemp);


                for (i = 0; i < intOccurences; i++)
                {
                    //APPENDS REPLACEWITH STRING
                    strOutput += replaceWith;

                    //FINDS POSITION OF NEXT SUBSTR
                    instNextPosition = IndexOfFixed(strStringTemp, strSubStrTemp, intCurrentPosition + strSubStrTemp.Length);

                    if (instNextPosition == -1)
                    {
                        //IE, IF THIS IS THE LAST ARM OF THE LOOP AND THERE ARE NO MORE SUBSTRINGS LEFT AFTER THIS IN MAIN SRING
                        //APPENDS THE REMAINDER OF THE MAIN STRING
                        strOutput += main.Substring(intCurrentPosition + sub.Length, main.Length - intCurrentPosition - sub.Length);
                    }
                    else
                    {
                        //GETS ALL TEXT UP TO THE NEXT strReplaceWith OCCURENCE AND APPENDS TO OUTPUT STRING
                        strOutput = strOutput + main.Substring(intCurrentPosition + sub.Length, instNextPosition - intCurrentPosition - replaceWith.Length);
                    }

                    //FIND POSITION IN STRING WHERE SUBSTR OCCURS
                    if (intCurrentPosition + replaceWith.Length >= strStringTemp.Length)
                        break;

                    intCurrentPosition = IndexOfFixed(strStringTemp, strSubStrTemp, intCurrentPosition + replaceWith.Length);


                }

            }
            else
            {
                //IF NO OCCURENCES OF SUBSTRING IN STRING, RETURNS UNMODIFIED STRING
                strOutput = main;
            }


            return strOutput;
        }


        /// <summary> Returns all in string after last occurrence of a substring</summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static string ReturnAfterLast(
            string main,
            string sub
            )
        {

            // if substring doesn't exist in main string, returns zero length string
            if (main.IndexOf(sub) == -1)
                return string.Empty;

            // if no text after substring, returns zero length string
            if (main.Length - 1 == main.LastIndexOf(sub))
                return string.Empty;

            // if reaches here, proceed to find desired substring
            return main.Substring(main.LastIndexOf(sub) + sub.Length, main.Length - main.LastIndexOf(sub) - sub.Length);
        }


        /// <summary> 
        /// Returns all text in a string until the first occurence of a substring 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static string ReturnUpto(
            string main,
            string sub
            )
        {
            int position;

            if ((main.Length == 0) || (sub.Length == 0) || (main.IndexOf(sub) == -1))
                return string.Empty;


            position = IndexOfFixed(main, sub);
            main = main.Substring(0, position);
            return main;
        }


        /// <summary> 
        /// Counts how many times a substring occurs in a string 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="stringsub"></param>
        /// <returns></returns>
        public static int StringCount(
            string main,
            string sub
            )
        {
            int i = 0;

            // simple "error" catching
            // exits function if either strMainString or strSubString are zero length, of if strSubString does not occur
            // in strMainString

            if (main.Length == 0)
                return 0;

            if (sub.Length == 0)
                return 0;

            if (main.IndexOf(sub) == -1)
                return 0;

            // loops through strMainString counting how many times strSubString occurs
            while (main.IndexOf(sub) != -1)
            {
                i++;
                main = main.Substring(IndexOfFixed(main, sub) + sub.Length, main.Length - sub.Length - IndexOfFixed(main, sub));
            }


            return i;
        }


    }
}

