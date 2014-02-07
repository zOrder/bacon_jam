using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NotificationManager
{
	public delegate void NotificationDelegate( Notification notification );

	private static NotificationManager instance;
	private Dictionary<string, List<NotificationDelegate>> observerTable;

	public static NotificationManager Instance
	{
		get { return instance ?? (instance = new NotificationManager()); }
	}

	private NotificationManager()
	{
		observerTable = new Dictionary<string, List<NotificationDelegate>>();
	}

	public void AddObserver(NotificationDelegate notificationDelegate, string notificationName)
	{
		List<NotificationDelegate> observerList = GetObserverForName(notificationName);
		observerList.Add(notificationDelegate);
	}

	public void RemoveObserver(NotificationDelegate notificationDelegate, string notificationName)
	{
		List<NotificationDelegate> observerList = GetObserverForName(notificationName);
		if(observerList.Contains(notificationDelegate))
		{
			observerList.Remove(notificationDelegate);
		}
	}

	public void PostNotification(string notificationName)
	{
		PostNotification(notificationName, Notification.Empty);
	}

	public void PostNotification( string notificationName, Notification notification)
	{
		List<NotificationDelegate> observerList = GetObserverForName(notificationName);
		observerList.ForEach( d => d(notification) );
	}
	
	private List<NotificationDelegate> GetObserverForName(string name)
	{
		if(!this.observerTable.ContainsKey(name))
		{
			this.observerTable.Add(name, new List<NotificationDelegate>());
		}
		return observerTable[name];
	}
}
