mergeInto(LibraryManager.library, {

    getHost: function () {
        return location.protocol + "//" + location.hostname;
    },
    getAPIHost: function () {
        var returnStr = location.protocol + "//" + "api." + location.hostname;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    getLanguage: function (){
        var returnStr = location.pathname.split('/', 2)[1];
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    getToken: function (){
        var returnStr = location.pathname.split('/', 2)[2];
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
});