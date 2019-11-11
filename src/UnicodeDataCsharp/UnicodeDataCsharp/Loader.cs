using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnicodeDataCsharp
{
    public class Loader
    {
        /// <summary>
        /// 指定した URL から取ってきたテキストデータをローカルにキャッシュする。
        /// キャッシュ読み書きは起動パス (. で取れるフォルダー)しか想定してない。
        /// </summary>
        /// <param name="url">大元の読み込み元 URL。</param>
        /// <param name="path">キャッシュ先。ここにファイルがあれば優先的にそれを読む。ないときだけ <paramref name="url"/> にアクセス。</param>
        public static async Task<byte[]> LoadContentAsync(string url, string path)
        {
            if (File.Exists(path))
            {
                return await File.ReadAllBytesAsync(path);
            }

            // 16.3 で Dispose 忘れの Messages が出るようになったんだけど…
            // HttpClient ってむしろ Dispose した方が遅いみたいな話なかったっけ…
            using var c = new HttpClient();
            var res = await c.GetAsync(url);
            var content = await res.Content.ReadAsByteArrayAsync();

            await File.WriteAllBytesAsync(path, content);

            return content;
        }
    }
}
