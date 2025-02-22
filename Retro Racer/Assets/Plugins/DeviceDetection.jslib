var DeviceDetection = {
    IsMobile: function() {
        if (/Mobi|Android|iPhone|iPad|iPod|Windows Phone/i.test(navigator.userAgent)) {
            return 1; // Mobile detected
        }
        return 0; // PC detected
    }
};

mergeInto(LibraryManager.library, DeviceDetection);
