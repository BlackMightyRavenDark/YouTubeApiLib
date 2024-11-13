
namespace YouTubeApiLib
{
	public interface IYouTubeMediaTrackUrlDecryptor
	{
		(bool, bool) Decrypt(YouTubeMediaTrackUrl mediaTrackUrl);
		bool DecryptNParam(string encryptedNParam, out string decryptionResult);
		bool DecryptCipherSignature(string encryptedCipherSignature, out string decryptionResult);
	}
}
