
using System;

namespace ProcrastinHater.BusinessInterfaces
{
	/// <summary>
	/// Interface for CRUD operations.
	/// </summary>
	public interface ICrudCapable<T>
	{
		T GetItemById(int id);
		bool AddItem(T newItem, out string err);
		bool DeleteItem(int id, out string err);
		bool UpdateItem(int id, T newItem, out string err);
				
	}
}
