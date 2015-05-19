//
//  FirstScrollViewFragment.cs
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
using Android.OS;
using Android.Views;
using Android.Widget;

namespace ParallaxHeaderviewpager
{
	public class FirstScrollViewFragment : ScrollTabHolderFragment, NotifyingScrollView.OnScrollChangedListener
	{
		public static string TAG = typeof(FirstScrollViewFragment).Name;
		private static int NO_SCROLL_X = 0;
		private static string ARG_POSITION = "position";

		private NotifyingScrollView mScrollView;
		private int mPosition;

		public static Fragment NewInstance(int position) {
			FirstScrollViewFragment fragment = new FirstScrollViewFragment();
			Bundle args = new Bundle();
			args.PutInt(ARG_POSITION, position);
			fragment.Arguments = args;
			return fragment;
		}

		public FirstScrollViewFragment() {}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container,	Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.fragment_first_scroll_view, container, false);
			mScrollView = (NotifyingScrollView) view.FindViewById(Resource.Id.scrollview);
			mScrollView.SetOnScrollChangedListener (this);

			mPosition = Arguments.GetInt(ARG_POSITION);
			return view;
		}

		public void onScrollChanged(ScrollView view, int l, int t, int oldl, int oldt) {

			if (mScrollTabHolder != null) {
				mScrollTabHolder.OnScrollViewScroll(view, l, t, oldl, oldt, mPosition);
			}
		}

		public void AdjustScroll(int scrollHeight, int headerTranslationY) {
			if (mScrollView == null) return;

			mScrollView.ScrollTo(NO_SCROLL_X, headerTranslationY - scrollHeight);
		}
	}
}

