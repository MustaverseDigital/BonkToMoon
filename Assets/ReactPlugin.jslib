mergeInto(LibraryManager.library, {
  SendMessageToReact: function(strPtr) {
    const message = Pointer_stringify(strPtr);
    // 定義全域函數接住這訊息
    if (window.onUnityMessage) {
      window.onUnityMessage(message);
    }
  }
});