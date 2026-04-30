using System;
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
                object? vm = ViewModelLocator.Resolve(view.GetType());
                if (vm != null)
                {
                    view.DataContext = vm;
                }
            }
        }

        /// <summary>
        /// 📌 ViewModel 타입에 대응되는 View를 동적으로 생성하고, DataContext를 설정하여 반환합니다.
        ///
        /// ViewModel의 네임스페이스 및 클래스명을 기준으로, 대응하는 View의 타입을 추론합니다.
        /// 예: <c>MainPageViewModel → MainPageView</c>, <c>ViewModels → Views</c> 네임스페이스 변환
        /// </summary>
        /// <param name="viewModel">ViewModel 인스턴스</param>
        /// <returns>생성된 View 인스턴스 (FrameworkElement)</returns>
        /// <exception cref="ArgumentNullException">viewModel이 null인 경우</exception>
        /// <exception cref="InvalidOperationException">View 타입을 찾을 수 없는 경우</exception>
        public static FrameworkElement ResolveView(object viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            Type vmType = viewModel.GetType();
            string fullName = vmType.FullName
                ?? throw new InvalidOperationException($"타입 전체 이름을 확인할 수 없습니다: {vmType.Name}");

            string[] candidates = ViewNamingConvention.GetViewTypeNameCandidates(fullName);
            Type? viewType = FindViewType(candidates);

            if (viewType == null)
            {
                throw new InvalidOperationException($"❌ View를 찾을 수 없습니다: {string.Join(" or ", candidates)}");
            }

            FrameworkElement view = (FrameworkElement)Activator.CreateInstance(viewType)!;
            view.DataContext = viewModel;

            return view;
        }

        /// <summary>
        /// 후보 이름 목록을 기준으로 현재 AppDomain에서 View 타입을 검색합니다.
        /// </summary>
        /// <param name="candidateTypeNames">검색할 View 타입 이름 후보 목록</param>
        /// <returns>검색된 View 타입, 없으면 null</returns>
        private static Type? FindViewType(string[] candidateTypeNames)
        {
            foreach (string candidateTypeName in candidateTypeNames)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type? viewType = assembly.GetType(candidateTypeName);
                    if (viewType == null)
                    {
                        continue;
                    }

                    if (!typeof(FrameworkElement).IsAssignableFrom(viewType))
                    {
                        continue;
                    }

                    if (viewType.IsAbstract)
                    {
                        continue;
                    }

                    return viewType;
                }
            }

            return null;
        }
    }
}