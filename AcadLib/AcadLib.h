// ClassLibrary1.h

#pragma once
#include "dbents.h"

using namespace System;
#ifdef NCAD
using namespace Teigha::DatabaseServices;
#else
using namespace Autodesk::AutoCAD::DatabaseServices;
#endif

namespace AcadLib {

	public ref class ObjectARX
	{
	public:

		static ResultBuffer^ _acdbEntGet(UIntPtr name)
		{
			AcDbObjectId oid;
			oid.setFromOldId(Adesk::UIntPtr(name));
			ads_name n;
			acdbGetAdsName(n, oid);
#ifdef NCAD
			OdResBufPtr resbufptr = oddbEntGet(oid);
		
			return ResultBuffer::Create(System::IntPtr(resbufptr.get()), true);
#else
			resbuf *buf = ::acdbEntGet(n);
			return ResultBuffer::Create(System::IntPtr(buf), true);
#endif
		}
	};
}
