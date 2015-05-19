using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Java.Lang;
using Android.Util;

namespace ParallaxHeaderviewpager
{
	[Activity (Label = "ParallaxHeaderviewpager", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FragmentActivity, ScrollTabHolder, Android.Support.V4.View.ViewPager.IOnPageChangeListener
	{
		private static string IMAGE_TRANSLATION_Y = "image_translation_y";
		private static string HEADER_TRANSLATION_Y = "header_translation_y";

		private ViewPager mViewPager;
		private View mHeader;
		private ImageView mTopImage;
		private CustomSlidingTabIndicator mNavigBar;
		private ViewPagerAdapter mAdapter;

		private int mMinHeaderHeight;
		private int mHeaderHeight;
		private int mMinHeaderTranslation;
		private int mNumFragments;

		private int mScrollState;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);

			InitValues();

			mTopImage = FindViewById<ImageView>(Resource.Id.image);
			mViewPager = FindViewById<ViewPager>(Resource.Id.view_pager);
			mNavigBar = FindViewById<CustomSlidingTabIndicator>(Resource.Id.navig_tab);
			mHeader = FindViewById(Resource.Id.header);

			if (bundle != null) {
				mTopImage.TranslationY = bundle.GetFloat(IMAGE_TRANSLATION_Y);
				mHeader.TranslationY = bundle.GetFloat(HEADER_TRANSLATION_Y);
			}

			SetupAdapter();
		}

		private void InitValues() {
			int tabHeight =Resources.GetDimensionPixelSize(Resource.Dimension.tab_height);
			mMinHeaderHeight = Resources.GetDimensionPixelSize(Resource.Dimension.min_header_height);
			mHeaderHeight = Resources.GetDimensionPixelSize(Resource.Dimension.header_height);
			mMinHeaderTranslation = - mMinHeaderHeight + tabHeight;

			mNumFragments = 3;
		}

		protected override void OnSaveInstanceState(Bundle outState) {
			outState.PutFloat(IMAGE_TRANSLATION_Y, mTopImage.TranslationY);
			outState.PutFloat(HEADER_TRANSLATION_Y, mHeader.TranslationY);
			base.OnSaveInstanceState(outState);
		}

		private void SetupAdapter() {
			if (mAdapter == null) {
				mAdapter = new ViewPagerAdapter(SupportFragmentManager, mNumFragments);
			}

			mViewPager.Adapter = mAdapter;
			mViewPager.OffscreenPageLimit = mNumFragments;
			mNavigBar.SetPageListener(this);
			mNavigBar.SetViewPager(mViewPager);
		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
			int currentItem = mViewPager.CurrentItem;
			if (positionOffsetPixels > 0) {
				SparseArray<ScrollTabHolder> scrollTabHolders = mAdapter.GetScrollTabHolders();

				ScrollTabHolder fragmentContent;
				if (position < currentItem) {
					// Revealed the previous page
					fragmentContent = scrollTabHolders.ValueAt(position);
				} else {
					// Revealed the next page
					fragmentContent = scrollTabHolders.ValueAt(position + 1);
				}

				fragmentContent.AdjustScroll((int) (mHeader.Height + mHeader.TranslationY),
					mHeader.Height);
			}
		}

		public void OnPageSelected(int position) {
			SparseArray<ScrollTabHolder> scrollTabHolders = mAdapter.GetScrollTabHolders();

			if (scrollTabHolders == null || scrollTabHolders.Size() != mNumFragments) {
				return;
			}

			ScrollTabHolder currentHolder = scrollTabHolders.ValueAt(position);
			currentHolder.AdjustScroll(
				(int) (mHeader.Height + mHeader.TranslationY),
				mHeader.Height);
		}

		public void OnPageScrollStateChanged(int state) {
			mScrollState = state;
		}

		public void AdjustScroll(int scrollHeight, int headerTranslationY) {}

		public void OnListViewScroll(AbsListView view, int firstVisibleItem, int visibleItemCount,
			int totalItemCount, int pagePosition) {
			if (mViewPager.CurrentItem == pagePosition) {
				ScrollHeader(GetScrollY(view));
			}
		}

		public void OnScrollViewScroll(ScrollView view, int x, int y, int oldX, int oldY, int pagePosition) {
			if (mViewPager.CurrentItem == pagePosition){
				ScrollHeader(view.ScrollY);
			}
		}

		private void ScrollHeader(int scrollY) {
			float translationY = Java.Lang.Math.Max(-scrollY, mMinHeaderTranslation);
			mHeader.TranslationY = translationY;
			mTopImage.TranslationY = -translationY / 3;
		}

		private int GetScrollY(AbsListView view) {
			View child = view.GetChildAt(0);
			if (child == null) {
				return 0;
			}

			int firstVisiblePosition = view.FirstVisiblePosition;
			int top = child.Top;

			int headerHeight = 0;
			if (firstVisiblePosition >= 1) {
				headerHeight = mHeaderHeight;
			}

			return -top + firstVisiblePosition * child.Height + headerHeight;
		}

		public class ViewPagerAdapter : FragmentPagerAdapter {

			private SparseArray<ScrollTabHolder> mScrollTabHolders;
//			private Android.Support.V4.Util.SparseArrayCompat<ScrollTabHolder> mScrollTabHolders;
			private int mNumFragments;

			public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm, int numFragments):base(fm) {
//				mScrollTabHolders = new Android.Support.V4.Util.SparseArrayCompat<ScrollTabHolder>();
				mScrollTabHolders = new SparseArray<ScrollTabHolder>();
				mNumFragments = numFragments;
			}

			public override Android.Support.V4.App.Fragment GetItem(int position) {
				Android.Support.V4.App.Fragment fragment;
				switch (position) {
				case 0:
					fragment = FirstScrollViewFragment.NewInstance(0);
					break;

				case 1:
//					fragment = SecondScrollViewFragment.newInstance(1);
					fragment = FirstScrollViewFragment.NewInstance(1);
					break;

				case 2:
					fragment = ListViewFragment.newInstance(2);
					break;

				default:
					throw new IllegalArgumentException("Wrong page given " + position);
				}
				return fragment;
			}

			public override Java.Lang.Object InstantiateItem(ViewGroup container, int position) {
				Java.Lang.Object obj = base.InstantiateItem(container, position);

				mScrollTabHolders.Put(position, (ScrollTabHolder) obj);

				return obj;
			}

			public override int Count {
				get {
					return mNumFragments;
				}
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted (int position) {
				switch (position) {
				case 0:
					return new Java.Lang.String ("ScrollView");

				case 1:
					return new Java.Lang.String ("ScrollView");

				case 2:
					return new Java.Lang.String ("ListView");

				default:
					throw new IllegalArgumentException("wrong position for the fragment in vehicle page");
				}
			}

			public override int GetItemPosition(Java.Lang.Object obj) {
				return PagerAdapter.PositionNone;
			}

			public SparseArray<ScrollTabHolder> GetScrollTabHolders() {
				return mScrollTabHolders;
			}
		}


	}
}


