/* functions called from unity */
mergeInto(LibraryManager.library, {
  FinishLoading: function() {
    document.dispatchEvent(new CustomEvent('Unity', {detail: 'Ready'}));
  },

  TestTimeReturn: function (texture) {
    document.dispatchEvent(new CustomEvent('Unity', {detail: 'Timer'}));
  },

  PostRender: function () {
    document.dispatchEvent(new CustomEvent('Unity', {detail: 'PostRender'}));
  },

  ConfigureToggleVRKeyName: function (keyName) {
    document.dispatchEvent(new CustomEvent('Unity', {detail: 'ConfigureToggleVRKeyName:' + Pointer_stringify(keyName)}));
  },

  ShowPanel: function (panelId) {
    document.dispatchEvent(new CustomEvent('Unity', {detail: {type: 'ShowPanel', panelId: Pointer_stringify(panelId)}}));
  },

  InitJavaScriptSharedArray: function(byteOffset, length) {
    console.log("initialized shared array");
    JavaScriptSharedArray = new Float32Array(buffer, byteOffset, length);

    // for (var i = 0; i < JavaScriptSharedArray.length; i++) {
    //   console.log(i + ': ' + JavaScriptSharedArray[i]);
    // }
  },

  ListenWebVRData: function() {
    document.addEventListener('VRData', function(evt) {
      var data = evt.detail;
      
      //var str = 'go: ';
      Object.keys(data).forEach(function (key, i) {
        
        //str += key + '/ ';
        var dataLength = data[key].length;
        for (var x = 0; x < dataLength; x++) { 
          JavaScriptSharedArray[i * dataLength + x] = data[key][x];
          //str += JavaScriptSharedArray[i + x] + '/ ';
        }
      });
      //console.log(str);
    });

      //console.log(str);
      
      // for (var i = 0; i < matVals.length; i++) {
      //   var matName = matVals[i];
        
      //   var matNamedValues = evt.detail[matName];
      //   for (var x = 0; x < 16; x++) {
      //     JavaScriptSharedArray[i + x] = matNamedValues[x];
      //   }
      // }
  }
});
