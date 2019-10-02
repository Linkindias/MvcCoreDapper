var keyCode = 'abcd1234EFGH0987';
var ivCode = 'ABCD7890efgh4321';
var ps = 'coremvc';

//加密
function Encrypt(source) {
    var key = CryptoJS.enc.Utf8.parse(keyCode);
    var iv = CryptoJS.enc.Utf8.parse(ivCode);
    var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(source), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
    return encrypted.toString();
}

//解碼
function Decrypt(encryptedData) {
    var decryptedText = null;
    var iv = CryptoJS.enc.Utf8.parse(ivCode);
    var Pass = CryptoJS.enc.Utf8.parse(ps);
    var Salt = CryptoJS.enc.Utf8.parse(keyCode);
    var key128Bits1000Iterations = CryptoJS.PBKDF2(Pass.toString(CryptoJS.enc.Utf8), Salt, { keySize: 128 / 32, iterations: 1000 });
    var cipherParams = CryptoJS.lib.CipherParams.create({
        ciphertext: CryptoJS.enc.Base64.parse(encryptedData)
    });

    var decrypted = CryptoJS.AES.decrypt(cipherParams, key128Bits1000Iterations, { mode: CryptoJS.mode.CBC, iv: iv, padding: CryptoJS.pad.Pkcs7 });
    decryptedText = decrypted.toString(CryptoJS.enc.Utf8);
    return decryptedText;
}