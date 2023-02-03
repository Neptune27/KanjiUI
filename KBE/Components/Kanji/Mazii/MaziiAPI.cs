//List<string> list = new () { "光" };
//var s = await KBE.WordController.FetchAll(list);
//int a = 0;

//string tmp;

//using (var sr = new StreamReader("..\\..\\..\\Test.txt"))
//{
//    // Read the stream as a string, and write the string to the console.
//    tmp = sr.ReadToEnd();
//}
//var a = JsonSerializer.Deserialize<KBE.MaziiAPI>(tmp);
////var d = JsonSerializer.Deserialize<KBE.MaziiAPIResults>(a.results[0].ToString());

namespace KBE.Components.Kanji.Mazii
{
    //enum KanjiDict
    //{
    //    KANJI,
    //    MEANING,
    //    ON,
    //    KUN,
    //    LEVEL,
    //    ENGLISH,
    //    VIETNAMESE,
    //    STROKES,
    //    PARTS,
    //    TAUGHT
    //}

    public class MaziiAPI
    {
        public int status { get; set; } = 400;
        public MaziiAPIResults[] results { get; set; } = new MaziiAPIResults[0];
    }
}
