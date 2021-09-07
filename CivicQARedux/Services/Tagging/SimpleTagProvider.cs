using CivicQARedux.Models.FormResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Services.Tagging
{
    public class SimpleTagProvider : ITagProvider
    {
        private readonly ISet<char> allowedChars;

        public SimpleTagProvider()
        {
            allowedChars = new HashSet<char>();

            for (char letter = 'a'; letter <= 'z'; letter++)
            {
                allowedChars.Add(letter);
            }
        }

        public Task<List<string>> GenerateTags(FormResponse response)
        {
            string fullText = response.Subject + " " + response.Body;
            fullText = fullText.ToLower();

            string[] words = fullText.Split(' ').ToArray();
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = new string(words[i]
                    .Where(c => allowedChars.Contains(c))
                    .ToArray());
            }

            return Task.FromResult(
                words
                .OrderByDescending(w => w.Length)
                .Take(3)
                .ToList()
            );
        }
    }
}
