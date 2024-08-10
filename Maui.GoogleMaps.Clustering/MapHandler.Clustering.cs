using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.GoogleMaps.Handlers;
{
	public partial class MapHandler
	{

		/// <summary>
		/// Call when before marker create.
		/// You can override your custom renderer for customize marker.
		/// </summary>
		/// <param name="outerItem">the pin.</param>
		/// <param name="innerItem">the marker options.</param>
		protected virtual void OnClusteredMarkerCreating(Pin outerItem, MarkerOptions innerItem)
		{
		}

		/// <summary>
		/// Call when after marker create.
		/// You can override your custom renderer for customize marker.
		/// </summary>
		/// <param name="outerItem">the pin.</param>
		/// <param name="innerItem">the clustered marker.</param>
		protected virtual void OnClusteredMarkerCreated(Pin outerItem, ClusteredMarker innerItem)
		{
		}

		/// <summary>
		/// Call when before marker delete.
		/// You can override your custom renderer for customize marker.
		/// </summary>
		/// <param name="outerItem">the pin.</param>
		/// <param name="innerItem">the clustered marker.</param>
		protected virtual void OnClusteredMarkerDeleting(Pin outerItem, ClusteredMarker innerItem)
		{
		}

		/// <summary>
		/// Call when after marker delete.
		/// You can override your custom renderer for customize marker.
		/// </summary>
		/// <param name="outerItem">the pin.</param>
		/// <param name="innerItem">the clustered marker.</param>
		protected virtual void OnClusteredMarkerDeleted(Pin outerItem, ClusteredMarker innerItem)
		{
		}
	}
}
