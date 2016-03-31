// ClassLibrary1.h

#pragma once
#include "dbents.h"


using namespace System;
using namespace Autodesk;


namespace AcadLib {

	public ref class ObjectARX
	{
		// TODO: здесь следует добавить свои методы для этого класса.
	public:

		static AutoCAD::DatabaseServices::ResultBuffer^ _acdbEntGet(UIntPtr name)
		{
			AcDbObjectId oid;
			oid.setFromOldId(Adesk::UIntPtr(name));
			ads_name n;
			acdbGetAdsName(n, oid);
			resbuf *buf = ::acdbEntGet(n);
			return AutoCAD::DatabaseServices::ResultBuffer::Create(IntPtr(buf), true);
		}
	};
}
