mergeInto(LibraryManager.library, {
  StartRank: function () {
    try {
      window.dispatchReactUnityEvent("StartRank");
    } catch (e) {
      console.warn("Failed to startGame event");
    }
  },
   EndGame: function (score) {
     try {
       window.dispatchReactUnityEvent("EndGame",score);
     } catch (e) {
       console.warn("Failed to endgame event");
     }
   },
});