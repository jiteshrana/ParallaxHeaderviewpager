//
//  ScrollTabHolderFragment.cs
//
//  Author:
//       welcome <>
//
//  Copyright (c) 2015 welcome
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Android.Support.V4.App;
using Android.App;
using Java.Lang;
using Android.Widget;

namespace ParallaxHeaderviewpager
{
	public class ScrollTabHolderFragment : Android.Support.V4.App.Fragment, ScrollTabHolder
	{
		protected ScrollTabHolder mScrollTabHolder;

		public override void OnAttach(Activity activity) {
			base.OnAttach(activity);
			try {
				mScrollTabHolder = (ScrollTabHolder) activity;
			}
			catch (ClassCastException e) {
				throw new ClassCastException(activity.ToString() + " must implement ScrollTabHolder");
			}
		}

		public override void OnDetach() {
			mScrollTabHolder = null;
			base.OnDetach();
		}

		public void AdjustScroll(int scrollHeight, int headerTranslationY) {}

		public void OnListViewScroll(AbsListView view, int firstVisibleItem, int visibleItemCount,
			int totalItemCount, int pagePosition) {}

		public void OnScrollViewScroll(ScrollView view, int x, int y, int oldX, int oldY, int pagePosition) {}
	}
}

