using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace thelab.core {

    /// <summary>
    /// Class that implements serialization methods for any System.Serializable class.
    /// </summary>    
    public class Serialize
    {
        /// <summary>
        /// Receives Base64 bytes and return a class T instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static public T FromBase64<T>(string p_data) {   
            return FromBytes<T>(Convert.FromBase64String(p_data));
        }

        /// <summary>
        /// Receives bytes and return a class T instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static public T FromBytes<T>(byte[] p_data) {
            BinaryFormatter bfmt = GetBinaryFormatter();                    
            MemoryStream stream  = new MemoryStream(p_data);
            T s = (T)bfmt.Deserialize(stream);
            stream.Close();
            return s;
        }

        /// <summary>
        /// Receives an json object as string and returns a class T instance. If the instance is an anonymous object template, it returns a new anonymous object with the filled data.
        /// If the 'populate' flag is true, the 'instance' will be filled with the json data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_data"></param>
        /// <param name="p_instance"></param>
        /// <param name="p_populate"></param>
        /// <returns></returns>
        static public T FromJson<T>(string p_data,T p_instance=default(T),bool p_populate=false) {
            if(p_instance==null) return JsonConvert.DeserializeObject<T>(p_data);
            if(p_populate) {
                JsonConvert.PopulateObject(p_data,p_instance);
                return p_instance;
            }
            return JsonConvert.DeserializeAnonymousType<T>(p_data,p_instance);
        }

        /// <summary>
        /// Receives an object and returns its binary serialization.
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static public byte[] ToBytes(object p_data) {
            if (!p_data.GetType().IsSerializable) {
                Debug.Log("Serialization> File not serializable!");
                return new byte[] {};
            }
            MemoryStream stream  = new MemoryStream();
            BinaryFormatter bfmt = GetBinaryFormatter();        
            bfmt.Serialize(stream, p_data);
            byte[] res = stream.ToArray();
            stream.Close();
            return res;
        }

        /// <summary>
        /// Receives an object and returns its binary serialization.
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static public string ToBase64(object p_data) {
            return Convert.ToBase64String(ToBytes(p_data));
        }

        /// <summary>
        /// Receives an object and returns its json serialization.
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static public string ToJson(object p_data,bool p_indented=false) {
            return JsonConvert.SerializeObject(p_data, p_indented ? Formatting.Indented : Formatting.None);
        }

        /// <summary>
        /// Returns a new instance of binary formatter.
        /// </summary>
        /// <returns></returns>
        static BinaryFormatter GetBinaryFormatter()
        {
            if(m_bfmt!=null) return m_bfmt;
            BinaryFormatter bfmt = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(Vector2),    new StreamingContext(StreamingContextStates.All), new Vector2SerializationSurrogate());
            ss.AddSurrogate(typeof(Vector3),    new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
            ss.AddSurrogate(typeof(Vector4),    new StreamingContext(StreamingContextStates.All), new Vector4SerializationSurrogate());
            ss.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerializationSurrogate());
            ss.AddSurrogate(typeof(Color),      new StreamingContext(StreamingContextStates.All), new ColorSerializationSurrogate());
            ss.AddSurrogate(typeof(Rect),       new StreamingContext(StreamingContextStates.All), new RectSerializationSurrogate());
            bfmt.SurrogateSelector = ss;
            return m_bfmt = bfmt;
        }
        static BinaryFormatter m_bfmt;

    }

    #region class Vector2SerializationSurrogate

    /// <summary>
    /// Workaround because unity don't serialize this struct.
    /// </summary>
    sealed class Vector2SerializationSurrogate : ISerializationSurrogate {

        // Method called to serialize a object
        public void GetObjectData(System.Object obj,SerializationInfo info, StreamingContext context) {
            Vector2 v = (Vector2)obj; info.AddValue("x", v.x); info.AddValue("y", v.y);
        }

        // Method called to deserialize a object
        public System.Object SetObjectData(System.Object obj,SerializationInfo info, StreamingContext context,ISurrogateSelector selector) {
            Vector2 v = (Vector2)obj; v.x = (float)info.GetValue("x", typeof(float)); v.y = (float)info.GetValue("y", typeof(float)); 
            return (obj = v);
        }
    }

    #endregion

    #region class Vector3SerializationSurrogate

    /// <summary>
    /// Workaround because unity don't serialize this struct.
    /// </summary>
    sealed class Vector3SerializationSurrogate : ISerializationSurrogate {

        // Method called to serialize a object
        public void GetObjectData(System.Object obj,SerializationInfo info, StreamingContext context) {
            Vector3 v = (Vector3)obj; info.AddValue("x", v.x); info.AddValue("y", v.y); info.AddValue("z", v.z);        
        }

        // Method called to deserialize a object
        public System.Object SetObjectData(System.Object obj,SerializationInfo info, StreamingContext context,ISurrogateSelector selector) {
            Vector3 v = (Vector3)obj; v.x = (float)info.GetValue("x", typeof(float)); v.y = (float)info.GetValue("y", typeof(float)); v.z = (float)info.GetValue("z", typeof(float));
            return (obj = v);
        }
    }

    #endregion

    #region class Vector4SerializationSurrogate

    /// <summary>
    /// Workaround because unity don't serialize this struct.
    /// </summary>
    sealed class Vector4SerializationSurrogate : ISerializationSurrogate {

        // Method called to serialize a object
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context) {
            Vector4 v = (Vector4)obj; info.AddValue("x", v.x); info.AddValue("y", v.y); info.AddValue("z", v.z); info.AddValue("w", v.w);
        }

        // Method called to deserialize a object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            Vector4 v = (Vector4)obj; v.x = (float)info.GetValue("x", typeof(float)); v.y = (float)info.GetValue("y", typeof(float)); v.z = (float)info.GetValue("z", typeof(float)); v.w = (float)info.GetValue("w", typeof(float));
            return (obj = v);
        }
    }

    #endregion

    #region class RectSerializationSurrogate

    /// <summary>
    /// Workaround because unity don't serialize this struct.
    /// </summary>
    sealed class RectSerializationSurrogate : ISerializationSurrogate {

        // Method called to serialize a object
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context) {
            Rect v = (Rect)obj; info.AddValue("xMin", v.xMin); info.AddValue("xMax", v.xMax); info.AddValue("yMin", v.yMin); info.AddValue("yMax", v.yMax);
        }

        // Method called to deserialize a object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            Rect v = (Rect)obj; v.xMin = (float)info.GetValue("xMin", typeof(float)); v.xMax = (float)info.GetValue("xMax", typeof(float)); v.yMin = (float)info.GetValue("yMin", typeof(float)); v.yMax = (float)info.GetValue("yMax", typeof(float));
            return (obj = v);
        }
    }

    #endregion

    #region class QuaternionSerializationSurrogate

    /// <summary>
    /// Workaround because unity don't serialize this struct.
    /// </summary>
    sealed class QuaternionSerializationSurrogate : ISerializationSurrogate {
        // Method called to serialize a object
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context) {
            Quaternion v = (Quaternion)obj; info.AddValue("x", v.x); info.AddValue("y", v.y); info.AddValue("z", v.z); info.AddValue("w", v.w);
        }

        // Method called to deserialize a object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            Quaternion v = (Quaternion)obj; v.x = (float)info.GetValue("x", typeof(float)); v.y = (float)info.GetValue("y", typeof(float)); v.z = (float)info.GetValue("z", typeof(float)); v.w = (float)info.GetValue("w", typeof(float));
            return (obj = v);
        }
    }

    #endregion

    #region class ColorSerializationSurrogate

    /// <summary>
    /// Workaround because unity don't serialize this struct.
    /// </summary>
    sealed class ColorSerializationSurrogate : ISerializationSurrogate {
        // Method called to serialize a object
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context) {
            Color v = (Color)obj; info.AddValue("r", v.r); info.AddValue("g", v.b); info.AddValue("b", v.b); info.AddValue("a", v.a);
        }

        // Method called to deserialize a Vector3 object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            Color v = (Color)obj; v.r = (float)info.GetValue("r", typeof(float)); v.g = (float)info.GetValue("g", typeof(float)); v.b = (float)info.GetValue("b", typeof(float)); v.a = (float)info.GetValue("a", typeof(float));
            obj = v;
            return obj;
        }
    }

    #endregion 

}