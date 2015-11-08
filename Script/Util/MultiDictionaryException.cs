using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace BK.Util
//{
    /// <summary>
    /// MultiMap collection's exception class;
    /// </summary>
    public class MultiDictionaryException: Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public MultiDictionaryException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExceptionParam"></param>
        /// <param name="ExMessage"></param>
        public MultiDictionaryException(Exception ExceptionParam, string ExMessage) : base(ExMessage, ExceptionParam) 
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public MultiDictionaryException(string  Message): base(Message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public MultiDictionaryException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

            
    }
//}
