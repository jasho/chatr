# Chat Screen Android Fix Plan

## Issues Found
1. **No auto-scroll** — after sending messages, CollectionView stays scrolled to top; latest message not visible
2. **Keyboard overlap** — Android default AdjustPan causes keyboard to cover compose row

## Fixes
### 1. Auto-scroll (ChatPage.xaml + .cs)
- Add x:Name="MessagesCollectionView" to CollectionView in XAML
- Subscribe to Messages.CollectionChanged in OnAppearing
- On Add: call ScrollTo(lastMessage, ScrollToPosition.End, animate: false)
- Unsubscribe in OnDisappearing

### 2. Keyboard resize (Platforms/Android/MainActivity.cs)
- Override OnCreate, call Window?.SetSoftInputMode(SoftInput.AdjustResize)
