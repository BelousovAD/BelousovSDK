mergeInto(LibraryManager.library, {
  IsMobile: function () {
    return typeof window>"u"||typeof navigator>"u"||typeof window.matchMedia>"u"?!1:window.matchMedia("(pointer: coarse)").matches;
  }
});