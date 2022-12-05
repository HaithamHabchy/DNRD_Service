using System;

namespace DNRD_Service.Utilities
{
    public class BAD_REQUEST_EXCEPTION : Exception
    {
        public BAD_REQUEST_EXCEPTION() : base() { }
    }

    
    public class UNAUTHORIZED_EXCEPTION : Exception
    {
        public UNAUTHORIZED_EXCEPTION() : base() { }
    }

    public class NO_DATA_FOUND_EXECEPTION : Exception
    {
        public NO_DATA_FOUND_EXECEPTION() : base() { }
    }

    public class HANDLED_EXCEPTION : Exception
    {
        public HANDLED_EXCEPTION() : base() { }
    }

}