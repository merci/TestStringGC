using UnityEngine;
using System;
using System.Collections.Generic;
using MyFramework;


public class Test : MonoBehaviour
{
    Dictionary<string, int> dict = new Dictionary<string, int>();

    ProfilerBlock profiler = ProfilerBlock.Instance;

    public string outsideString;
    public string outsideString1;
    string bigString= new string('x', 10000);

    public bool bigStringTest = false;

    void Start() 
    {
        using (zstring.Block())
        {
            zstring.zstring_block block01 = zstring.g_current_block;
            zstring.zstring_block block02;
            using (zstring.Block())
            {
                block02 = zstring.g_current_block;
            }
            zstring.zstring_block block03 = zstring.g_current_block;

            Debug.Log(object.ReferenceEquals(block01, block02));//False  False
            Debug.Log(object.ReferenceEquals(block01, block03));//False  True
            Debug.Log(object.ReferenceEquals(block02, block03));//True   false            
        }
   
        using (zstring.Block())
        {
            zstring a = "hello";
            zstring b = " 我曹の:-O";
            zstring c = a + b;
            zstring d = c + b;
            Debug.Log(d);
            Debug.Log(zstring.Format("aaa{0}{1}", "我曹", "喔"));    
            testSizeof02();
        }
    }


    unsafe void testSizeof02()
    {
        string s = "Assets/ResourcesAssets/Prefabs/Scene/Battle/battle003/tx_PVE_alpha_model_003.prefab";
        Debug.Log(s.Length);        
        Debug.Log(System.Text.Encoding.UTF8.GetBytes(s).Length);
        Debug.Log(s.Length * sizeof(char));
        fixed (char* sptr=s)
        {
            int startPos = (int)sptr;
            int endPos = (int)(sptr + s.Length);
            Debug.Log(endPos-startPos);
        }
    }
 
    void Update()
    {
        for (int n = 0; n < 1000; n++)
        {
            stringTest();//原生string
            zstringTest();//骚操作版nstring
        }
    }

    void stringTest()
    {
        using (profiler.Sample("string"))
        {
            using (profiler.Sample("Format"))
            {
                string gf = string.Format("Number = {0}, Float = {1} String = {2}", 123, 3.148f, "Text");

            }

            using (profiler.Sample("Concat"))
            {
                string it = string.Concat("That's ", "a lot ", " of", " strings", " to ", "concat");
            }

            using (profiler.Sample("Substring + IndexOf + LastIndexOf"))
            {
                string path = "Path/To/Some/File.txt";
                int period = path.IndexOf('.');
                var ext = path.Substring(period + 1);
                var file = path.Substring(path.LastIndexOf('/') + 1, 4);
               
            }

            using (profiler.Sample("Replace (char)"))
            {
                string input = "This is some sort of text";
                string replacement = input.Replace('o', 'O').Replace('i', 'I');
               
            }

            using (profiler.Sample("Replace (string)"))
            {
                string input = "m_This is the is is form of text";
                string replacement = input.Replace("m_", "").Replace("is", "si");
               
            }

            using (profiler.Sample("ToUpper + ToLower"))
            {
                string s1 = "Hello";
                string s2 = s1.ToUpper();
                string s3 = s2 + s1.ToLower();
               
            }
            if (!bigStringTest)
            {
                return;
            }
            using (profiler.Sample("BigStringEval"))
            {
                string s1 = bigString;
                string s2 = s1 + "hello";
            }
        }
    }

    void zstringTest()
    {
        using (profiler.Sample("zstring"))
        {
            using (zstring.Block())
            {
                using (profiler.Sample("Format"))
                {
                    zstring gf = zstring.Format("Number = {0}, Float = {1} String = {2}", 123, 3.148f, "Text");
                   
                }

                using (profiler.Sample("Concat"))
                {
                    zstring it = zstring.Concat("That's ", "a lot", " of", " strings", " to ", "concat");
                   
                }

                using (profiler.Sample("Substring + IndexOf + LastIndexOf"))
                {
                    zstring path = "Path/To/Some/File.txt";
                    int period = path.IndexOf('.');
                    var ext = path.Substring(period + 1);
                    var file = path.Substring(path.LastIndexOf('/') + 1, 4);
                   
                }

                using (profiler.Sample("Replace (char)"))
                {
                    zstring input = "This is some sort of text";
                    zstring replacement = input.Replace('o', '0').Replace('i', '1');
                   
                }

                using (profiler.Sample("Replace (string)"))
                {
                    zstring input = "m_This is the is is form of text";
                    zstring replacement = input.Replace("m_", "").Replace("is", "si");
                   
                }
                using (profiler.Sample("Concat + Intern"))
                {
                    for (int i = 0; i < 4; i++)
                        dict[zstring.Concat("Item", i).Intern()] = i;
                    outsideString1 = zstring.Concat("I'm ", "long ", "gone ", "by ", "the ", "end ", "of ", "this ", "gstring block");
                    outsideString = zstring.Concat("I'll ", "be ", "still ", "around ", "here").Intern();
                   
                }

                using (profiler.Sample("ToUpper + ToLower"))
                {
                    zstring s1 = "Hello";
                    zstring s2 = s1.ToUpper();
                    zstring s3 = s2 + s1.ToLower();
                   
                }

                if (!bigStringTest)
                {
                    return;
                }
                using (profiler.Sample("BigStringEval"))
                {
                    zstring s1 = bigString;
                    zstring s2 = s1 + "hello";
                }
            }
        }
    }
   

}
public class ProfilerBlock : IDisposable
{
    public static readonly ProfilerBlock Instance = new ProfilerBlock();

    public IDisposable Sample(string sample)
    {
        UnityEngine.Profiling.Profiler.BeginSample(sample);
        return this;
    }

    public void Dispose()
    {
        UnityEngine.Profiling.Profiler.EndSample();
    }
}