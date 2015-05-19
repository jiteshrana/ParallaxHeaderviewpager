//
//  ListViewFragment.cs
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
using Android.Widget;
using Android.Support.V4.App;
using Android.OS;
using Android.Views;

namespace ParallaxHeaderviewpager
{
	public class ListViewFragment : ScrollTabHolderFragment,AbsListView.IOnScrollListener
	{
		public ListViewFragment ()
		{
		}

		public static string TAG = typeof(ListViewFragment).Name;
		private static string ARG_POSITION = "position";

		private ListView mListView;
		private int mPosition;

		public static Fragment newInstance(int position) {
			ListViewFragment fragment = new ListViewFragment();
			Bundle args = new Bundle();
			args.PutInt(ARG_POSITION, position);
			fragment.Arguments = args;
			return fragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.fragment_list_view, container, false);
			mListView = view.FindViewById<ListView>(Resource.Id.listview);
			View placeHolderView = inflater.Inflate(Resource.Layout.header_placeholder, mListView, false);
			mListView.AddHeaderView(placeHolderView);

			mPosition = Arguments.GetInt(ARG_POSITION);

			SetAdapter();

			mListView.SetOnScrollListener (this);

			return view;
		}

		public  void OnScroll(AbsListView view, int firstVisible, int visibleCount, int totalCount)
		{
			if (mScrollTabHolder != null) {
				mScrollTabHolder.OnListViewScroll(
					view, firstVisible, visibleCount, totalCount, mPosition);
			}
		}

		public void OnScrollStateChanged(AbsListView v, ScrollState s){}

		private void SetAdapter() {
			if (Activity == null) return;

			int size = 50;
			String[] stringArray = new String[size];
			for (int i = 0; i < size; ++i) {
				stringArray[i] = ""+i;
			}

			ArrayAdapter<String> adapter =
				new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleListItem1, stringArray);

			mListView.Adapter = adapter;
		}

		public void AdjustScroll(int scrollHeight, int headerTranslationY) {
			if (mListView == null) return;

			if (scrollHeight == 0 && mListView.FirstVisiblePosition >= 1) {
				return;
			}

			mListView.SetSelectionFromTop(1, scrollHeight);
		}
	}
}

