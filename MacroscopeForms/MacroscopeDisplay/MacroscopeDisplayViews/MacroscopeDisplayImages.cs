﻿/*
	
	This file is part of SEOMacroscope.
	
	Copyright 2017 Jason Holland.
	
	The GitHub repository may be found at:
	
		https://github.com/nazuke/SEOMacroscope
	
	Foobar is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.
	
	Foobar is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.
	
	You should have received a copy of the GNU General Public License
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SEOMacroscope
{
	
	/// <summary>
	/// Description of MacroscopeDisplayImages.
	/// </summary>

	public class MacroscopeDisplayImages : MacroscopeDisplayListView
	{
		
		/**************************************************************************/

		static Boolean ListViewConfigured = false;
		
		/**************************************************************************/

		public MacroscopeDisplayImages ( MacroscopeMainForm MainFormNew, ListView lvListViewNew )
			: base( MainFormNew, lvListViewNew )
		{

			MainForm = MainFormNew;
			lvListView = lvListViewNew;
						
			if( MainForm.InvokeRequired )
			{
				MainForm.Invoke(
					new MethodInvoker (
						delegate
						{
							ConfigureListView();
						}
					)
				);
			}
			else
			{
				ConfigureListView();
			}

		}

		/**************************************************************************/
		
		void ConfigureListView ()
		{
			if( !ListViewConfigured )
			{
				ListViewConfigured = true;
			}
		}

		/**************************************************************************/

		protected override void RenderListView ( MacroscopeDocument msDoc, string sUrl )
		{

			if( !msDoc.GetIsImage() )
			{
				return;
			}

			string sStatusCode = msDoc.GetStatusCode().ToString();
			string sMimeType = msDoc.GetMimeType();
			string sFileSize = msDoc.GetContentLength().ToString();

			string sPairKey = string.Join( "", sUrl );

			ListViewItem lvItem = null;
							
			if( this.lvListView.Items.ContainsKey( sPairKey ) )
			{
							
				try
				{

					lvItem = this.lvListView.Items[ sPairKey ];
					lvItem.SubItems[ 0 ].Text = sUrl;
					lvItem.SubItems[ 1 ].Text = sStatusCode;
					lvItem.SubItems[ 2 ].Text = sMimeType;
					lvItem.SubItems[ 3 ].Text = sFileSize;

				}
				catch( Exception ex )
				{
					DebugMsg( string.Format( "MacroscopeDisplayImages 1: {0}", ex.Message ) );
				}

			}
			else
			{
							
				try
				{

					lvItem = new ListViewItem ( sPairKey );
					lvItem.UseItemStyleForSubItems = false;
					lvItem.Name = sPairKey;

					lvItem.SubItems[ 0 ].Text = sUrl;
					lvItem.SubItems.Add( sStatusCode );
					lvItem.SubItems.Add( sMimeType );
					lvItem.SubItems.Add( sFileSize );

					this.lvListView.Items.Add( lvItem );

				}
				catch( Exception ex )
				{
					DebugMsg( string.Format( "MacroscopeDisplayImages 2: {0}", ex.Message ) );
				}

			}

			if( lvItem != null )
			{

				lvItem.ForeColor = Color.Blue;
				
				if( msDoc.GetStatusCode() != 200 )
				{
					lvItem.SubItems[ 1 ].ForeColor = Color.Red;
				}
				else
				{
					lvItem.SubItems[ 1 ].ForeColor = Color.ForestGreen;
				}

			}
			
		}

		/**************************************************************************/

	}

}