using System;
using System.Collections.Generic;
using System.Text;

namespace Hanyang.BindingData
{
    public class Article
    {
        public string Title { get; set; } // 글 제목
        public string Info { get; set; } // 작성자, 작성 날짜 (ㅇㅇㅇ | 2020.06.23)

        public Article(string title, string info)
        {
            Title = title;
            Info = info;
        }
    }
}
