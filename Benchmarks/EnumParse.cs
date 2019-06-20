using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    [RyuJitX64Job]
    public class EnumParse
    {
        private const int TestCases = 1_000_000;
        string[] options = new[] { "Red", "Green", "Blue", "1", "2", "3", "unknown", "4", "123456" };
        string[] dataToConvert;
        Dictionary<string, Colour> mappingDict;

        [GlobalSetup]
        public void Setup()
        {
            var r = new Random();

            dataToConvert =
                Enumerable
                .Range(1, TestCases)
                .Select(_ => options[r.Next(options.Length)])
                .ToArray();

            mappingDict =
                Enum
                .GetValues(typeof(Colour))
                .Cast<Colour>()
                .SelectMany(v =>
                    new[] {
                                    (n: v.ToString(), v),
                                    (n: ((int)v).ToString(), v)
                })
                .ToDictionary(i => i.n, i => i.v, StringComparer.OrdinalIgnoreCase);
        }

        [Benchmark]
        public Colour[] TryParseAndIsDefined()
        {
            return dataToConvert
                .Select(startOfPath => {
                    if (Enum.TryParse(startOfPath, true, out Colour apiMethodGroupId) &&
                        Enum.IsDefined(typeof(Colour), apiMethodGroupId))
                        return apiMethodGroupId;

                    return (Colour)0;
                })
                .ToArray();
        }

        //[Benchmark]
        public Colour[] IsDefinedAndTryParse()
        {
            return dataToConvert
                .Select(startOfPath => {
                    if (Enum.IsDefined(typeof(Colour), startOfPath) &&
                        Enum.TryParse(startOfPath, true, out Colour apiMethodGroupId))
                        return apiMethodGroupId;

                    return (Colour)0;
                })
                .ToArray();
        }

        [Benchmark]
        public Colour[] TryParseOnly()
        {
            return dataToConvert
                .Select(startOfPath => {
                    if (Enum.TryParse(startOfPath, true, out Colour apiMethodGroupId))
                        return apiMethodGroupId;

                    return (Colour)0;
                })
                .ToArray();
        }

        [Benchmark]
        public Colour[] UsingDictionary()
        {
            return dataToConvert
                .Select(startOfPath => {
                    if (mappingDict.TryGetValue(startOfPath, out Colour apiMethodGroupId))
                        return apiMethodGroupId;

                    return (Colour)0;
                })
                .ToArray();
        }
    }

    public enum Colour
    {

        Red = 1,
        Green = 2,
        Blue = 3
    }
}
