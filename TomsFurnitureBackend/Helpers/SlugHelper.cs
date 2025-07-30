using System.Text;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;

namespace TomsFurnitureBackend.Helpers
{
    public static class SlugHelper
    {
        // Hàm tạo slug duy nhất, sử dụng delegate để kiểm tra tính duy nhất
        public static async Task<string> GenerateUniqueSlugAsync(
            string name,
            Func<string, Task<bool>> existsAsync,
            int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.");
            }

            // Chuyển tên thành slug
            string slug = ToSlug(name, maxLength);

            string baseSlug = slug;
            int counter = 1;

            // Kiểm tra tính duy nhất của slug
            while (await existsAsync(slug))
            {
                slug = $"{baseSlug}-{counter}";
                if (slug.Length > maxLength)
                {
                    slug = $"{baseSlug.Substring(0, maxLength - counter.ToString().Length - 1)}-{counter}";
                }
                counter++;
            }

            return slug;
        }

        // Hàm chuyển tên thành slug (không dấu, viết thường, thay khoảng trắng bằng dấu gạch ngang)
        private static string ToSlug(string name, int maxLength)
        {
            // Chuyển thành viết thường và chuẩn hóa không dấu
            string normalized = name.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            string noAccents = normalized
                .Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                .ToString()
                .Normalize(NormalizationForm.FormC);

            // Chuyển sang ASCII để loại bỏ các ký tự đặc biệt còn lại
            string ascii = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(noAccents));

            // Chỉ giữ chữ cái, số, và thay khoảng trắng bằng dấu gạch ngang
            StringBuilder slugBuilder = new StringBuilder();
            bool lastWasSpace = true;

            foreach (char c in ascii)
            {
                if (char.IsLetterOrDigit(c))
                {
                    slugBuilder.Append(c);
                    lastWasSpace = false;
                }
                else if (c == ' ' && !lastWasSpace)
                {
                    slugBuilder.Append('-');
                    lastWasSpace = true;
                }
            }

            // Cắt bỏ dấu gạch ngang ở đầu hoặc cuối
            string slug = slugBuilder.ToString().Trim('-');

            // Giới hạn độ dài
            if (slug.Length > maxLength)
            {
                slug = slug.Substring(0, maxLength).Trim('-');
            }

            return string.IsNullOrEmpty(slug) ? "slug" : slug;
        }
    }
}