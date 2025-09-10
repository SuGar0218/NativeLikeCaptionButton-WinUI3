# Native-like Caption Button for WinUI 3

Provides flexible and customizable title bar for WinUI 3 in Windows 11 native UWP style.

<img width="1880" height="367" alt="image" src="https://github.com/user-attachments/assets/38ceb971-8c39-444e-8c17-1831e4a3f8bd" />

## Features

- Every caption button can be disabled or hidden.
- Title bar height can be set freely like many other xaml controls.
- Supports custom content in title bar.

<img width="3885" height="1053" alt="image" src="https://github.com/user-attachments/assets/30687574-0b63-48bf-91d5-d060eb26e723" />

<img width="3653" height="289" alt="image" src="https://github.com/user-attachments/assets/30e6d3d1-8e8a-43c5-92ce-25b12da71c82" />

## Drawbacks

- Does not support Snap Layout on Windows 11.

<img width="3756" height="872" alt="image" src="https://github.com/user-attachments/assets/e7c4a38e-79b9-451e-b169-f34fed16ecd9" />

## How to use

Remember to set OwnerWindow to ensure it can change the window state properly.

Here is part of the code from the sample app.

``` xaml
<my:NativeLikeTitleBar
    Height="42"
    BackButtonVisibility="{x:Bind ViewModel.IsBackButtonVisible, Converter={StaticResource BoolToVisibilityConverter}}"
    CloseButtonVisibility="{x:Bind ViewModel.IsCloseButtonVisible, Converter={StaticResource BoolToVisibilityConverter}}"
    IsBackButtonEnabled="{x:Bind ViewModel.IsBackButtonEnabled}"
    IsCloseButtonEnabled="{x:Bind ViewModel.IsCloseButtonEnabled}"
    IsMaximizeButtonEnabled="{x:Bind ViewModel.IsMaximizeButtonEnabled}"
    IsMinimizeButtonEnabled="{x:Bind ViewModel.IsMinimizeButtonEnabled}"
    IsPaneToggleButtonEnabled="{x:Bind ViewModel.IsPaneToggleButtonEnabled}"
    MaximizeButtonVisibility="{x:Bind ViewModel.IsMaxmizeButtonVisible, Converter={StaticResource BoolToVisibilityConverter}}"
    MinimizeButtonVisibility="{x:Bind ViewModel.IsMinmizeButtonVisible, Converter={StaticResource BoolToVisibilityConverter}}"
    OwnerWindow="{x:Bind OwnerWindow}"
    PaneToggleButtonClick="TitleBarAreaPaneToggleButtonClick"
    PaneToggleButtonVisibility="{x:Bind ViewModel.IsPaneToggleButtonVisible, Converter={StaticResource BoolToVisibilityConverter}}">

    <my:NativeLikeTitleBar.CustomHeader>
        <!-- Place custom header content here (on the right of back button and pane toggle button) -->
    </my:NativeLikeTitleBar.CustomHeader>

    <my:NativeLikeTitleBar.CustomFooter>
        <!-- Place footer header content here (on the left of caption buttons) -->
    </my:NativeLikeTitleBar.CustomFooter>

    <!-- Place content in the center of title bar here (on the left of caption buttons) -->
</my:NativeLikeTitleBar>
```

You can also try it in the sample app.

<img width="3040" height="1798" alt="image" src="https://github.com/user-attachments/assets/ace75151-331d-4e32-ad7c-c36f84014d29" />
