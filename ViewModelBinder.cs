using System.Windows;
using Dreamine.MVVM.Locators;

namespace Dreamine.MVVM.Locators.Wpf
{
	/// <summary>
	/// 📌 XAML에서 ViewModel을 자동으로 바인딩하기 위한 WPF 전용 헬퍼 클래스입니다.
	///
	/// 이 클래스는 WPF의 AttachedProperty를 사용하여,
	/// View에 ViewModel을 네이밍 규칙에 따라 자동으로 연결합니다.
	///
	/// 현재는 사용되지 않지만, XAML 선언적 바인딩이 필요할 경우를 대비하여 보존됩니다.
	/// 예: <code>&lt;Window vm:ViewModelBinder.AutoWireViewModel="True" /&gt;</code>
	/// </summary>
	public static class ViewModelBinder
	{
		/// <summary>
		/// 📌 ViewModel 자동 연결을 위한 AttachedProperty입니다.
		///
		/// View의 클래스명에 기반하여 ViewModelLocator를 통해 ViewModel을 생성하고,
		/// 해당 View의 DataContext에 자동으로 바인딩합니다.
		/// </summary>
		public static readonly DependencyProperty AutoWireViewModelProperty =
			DependencyProperty.RegisterAttached(
				"AutoWireViewModel",
				typeof(bool),
				typeof(ViewModelBinder),
				new PropertyMetadata(false, OnAutoWireViewModelChanged));

		/// <summary>
		/// AttachedProperty의 getter입니다.
		/// </summary>
		/// <param name="obj">DependencyObject 대상</param>
		/// <returns>자동 바인딩 여부 (true/false)</returns>
		public static bool GetAutoWireViewModel(DependencyObject obj)
			=> (bool)obj.GetValue(AutoWireViewModelProperty);

		/// <summary>
		/// AttachedProperty의 setter입니다.
		/// </summary>
		/// <param name="obj">DependencyObject 대상</param>
		/// <param name="value">true 설정 시 ViewModel 자동 연결을 시도합니다.</param>
		public static void SetAutoWireViewModel(DependencyObject obj, bool value)
			=> obj.SetValue(AutoWireViewModelProperty, value);

		/// <summary>
		/// Property 값 변경 시 호출되는 콜백 메서드입니다.
		/// View의 타입을 기반으로 ViewModel을 찾아 DataContext에 할당합니다.
		/// </summary>
		/// <param name="d">연결 대상 객체</param>
		/// <param name="e">속성 변경 정보</param>
		private static void OnAutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is FrameworkElement view && (bool)e.NewValue)
			{
				var vm = ViewModelLocator.Resolve(view.GetType());
				if (vm != null)
					view.DataContext = vm;
			}
		}
	}
}
