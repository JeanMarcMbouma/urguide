export function BlobToBase64(blob: File, callback: (fileName: string, base64Url: any, blobUrl: string) => { }): void {
    var fileName = blob.name;
    var filePath = URL.createObjectURL(blob);
    var reader = new window.FileReader();
    reader.readAsDataURL(blob);
    reader.onloadend = function () {
        const base64data = reader.result;
        callback(fileName, base64data, filePath);
    }
}