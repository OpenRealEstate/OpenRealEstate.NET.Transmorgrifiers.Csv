using System;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv.Tests
{
    public class FakeListing : Listing
    {
        public override Core.Listing ToOreListing()
        {
            throw new NotImplementedException();
        }
    }
}