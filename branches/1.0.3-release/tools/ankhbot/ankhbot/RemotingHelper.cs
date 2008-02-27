//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AnkhBot
//{
//    public class RemotingHelper
//    {
		

//        public static Object GetObject( Type type )
//        {
//            if (!isInit) 
//                InitTypeCache();

//            WellKnownClientTypeEntry entr = wellKnownTypes[type];

//            if (entr == null)
//            {
//                throw new RemotingException( "Type not found!" );
//            }

//            return Activator.GetObject( entr.ObjectType, entr.ObjectUrl );
//        }

//        public static void InitTypeCache()
//        {
//            wellKnownTypes = new Hashtable();
//            foreach (WellKnownClientTypeEntry entr in
//                RemotingConfiguration.GetRegisteredWellKnownClientTypes())
//            {
//                wellKnownTypes.Add( entr.ObjectType, entr );
//            }
//            isInit = true;
//        }

//        private static bool isInit;
//        private static Dictionary<Type, WellKnownClientTypeEntry> wellKnownTypes;
//    }
//}
