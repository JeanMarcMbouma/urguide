"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function BlobToBase64(blob, callback) {
    var fileName = blob.name;
    var filePath = URL.createObjectURL(blob);
    var reader = new window.FileReader();
    reader.readAsDataURL(blob);
    reader.onloadend = function () {
        var base64data = reader.result;
        callback(fileName, base64data, filePath);
    };
}
exports.BlobToBase64 = BlobToBase64;
//# sourceMappingURL=fileHelpers.js.map