# **한양이 (Hanyang)**

> ## **목차**
>
> 1. [한양이란?](#한양이란)
> 2. [작동 원리](#작동-원리)
>    1. [시간표, 급식 메뉴, 학사 일정](#시간표-급식-메뉴-학사-일정)
>    2. [공지사항, 가정통신문](#공지사항-가정통신문)
> 3. [스크린샷 (시연 영상)](#스크린샷-시연-영상)
> 4. [개발 정보](#개발-정보)
> 5. [배포 방법 (Lightsail 기준)](#배포-방법-Lightsail-기준)

---

> ### **한양이란?**
>
> - 한양공업고등학교의 학년별(과별) 시간표와 그 달의 급식 메뉴, 1년간의 학사 일정을 보여줍니다.
> - [한양공업고등학교 홈페이지](http://hanyang.sen.hs.kr/index.do)의 공지사항, 가정통신문을 가져와 보여줍니다.
> - 앱에서 [[한양공업고등학교 홈페이지](http://hanyang.sen.hs.kr/index.do) • [한양뉴스](http://www.hanyangnews.com/) • [코로나맵](https://coronamap.site/) • [건강상태 자가진단](https://hcs.eduro.go.kr/#/loginWithUserInfo)] 사이트로 바로 이동이 가능합니다.

---

> ### **작동 원리**
>
> - #### **시간표, 급식 메뉴, 학사 일정**
>
>   - [나이스 오픈 API](https://open.neis.go.kr/portal/guide/apiIntroPage.do)를 이용하여 서버로 데이터를 가져옵니다.
>
> - #### **공지사항, 가정통신문**
>   - 셀레니움(Selenium)을 이용하여 [한양공업고등학교 홈페이지](http://hanyang.sen.hs.kr/index.do)의 공지사항, 가정통신문 게시글을 크롤링 하여 서버로 데이터를 가져옵니다.
>
> 가져온 모든 데이터는 서버에서 가공되고, RESTful API를 이용하여 앱(클라이언트)에서 서버로부터 데이터를 가져온 뒤 사용자에게 보여줍니다.

---

> ### **스크린샷 (시연 영상)**
>
> - 스크린샷  
>   ![한양이 스크린샷1](https://github.com/banb3515/Hanyang/blob/master/Screenshots/Screenshot1.png?raw=true)  
>   ![한양이 스크린샷2](https://github.com/banb3515/Hanyang/blob/master/Screenshots/Screenshot2.png?raw=true)
> - 시연 영상  
>   [![한양이 시연 영상](https://github.com/banb3515/Hanyang/blob/master/Screenshots/Thumbnail.png?raw=true)](https://youtu.be/2rMCJMG5Ohc?t=0s)

---

> ### **개발 정보**
>
> - 개발 언어 - [C#](https://docs.microsoft.com/ko-kr/dotnet/csharp/)
> - 앱 프레임워크 - [자마린(Xamarin)](https://docs.microsoft.com/ko-kr/xamarin/get-started/what-is-xamarin)
> - 웹서버 프레임워크 - [ASP.NET](https://dotnet.microsoft.com/apps/aspnet)
> - 사용된 패키지
>   - [AiForms.Dialogs](https://github.com/muak/AiForms.Dialogs)
>   - [ByteSize](https://github.com/omar/ByteSize)
>   - [HtmlAgilityPack](https://html-agility-pack.net/)
>   - [Newtonsoft.Json](https://www.newtonsoft.com/json)
>   - [Rg.Plugins.Popup](https://github.com/rotorgames/Rg.Plugins.Popup)
>   - [Selenium](https://www.selenium.dev/)
>   - [Syncfusion.Xamarin.SfCalendar](https://www.syncfusion.com/)

---

> ### **배포 방법 (Lightsail 기준)**
>
> 1. [Lightsail 인스턴스 생성](https://lightsail.aws.amazon.com/ls/webapp/home/instances)
> 2. [Lightsail 자습서](https://aws.amazon.com/ko/getting-started/hands-on/host-net-web-app/)
> 3. 서버에 [Windows Hosting Bundle Installer](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.8-windows-hosting-bundle-installer) 설치
> 4. 서버에 [Java SE Runtime Environment 8](https://www.java.com/ko/download/) 설치
> 5. 서버에 [Chrome](https://www.google.com/chrome/) 설치
> 6. 서버에 [Selenium Server](https://www.selenium.dev/downloads/), [Chrome Driver](https://sites.google.com/a/chromium.org/chromedriver/downloads) 설치
> 7. Selenium Server 파일과 Chrome Driver를 같은 경로에 두고 Selenium Server 실행 **[CMD 창 > java -jar {File Name}.jar]**
> 8. 공용 프로젝트 > App.xaml.cs > #region 변수
>
>    ```
>    // 한양이 WebServer API 키
>    // ex) public const string API_KEY = "IWcusufuaYOcq5t/8tXC+6cJcrF5Y8zjoS915vG47yo=";
>    public const string API_KEY = "{YOUR_API_KEY}"; // 변경 - 서버 키 값과 동일해야함
>
>    // 서버 URL
>    // ex) public static string ServerUrl { get; } = "http://3.34.53.147/";
>    public static string ServerUrl { get; } = "http://{YOUR_SERVER_DOMAIN}/"; // 변경
>    ```
>
> 9. WebServer 프로젝트 > Program.cs > #region 변수
>
>    ```
>    // 한양이 WebServer API 키
>    // ex) public const string API_KEY = "IWcusufuaYOcq5t/8tXC+6cJcrF5Y8zjoS915vG47yo=";
>    public const string API_KEY = "{YOUR_API_KEY}"; // 변경 - 클라이언트 키 값과 동일해야함
>
>    // NEIS API -> API 키
>    // ex) private const string NEIS_API_KEY = "KEY=762281280e4943e58669a6b02991a67a&";
>    private const string NEIS_API_KEY = "KEY={YOUR_NEIS_API_KEY}&"; // 나이스 Open API 사이트에서 발급받은 API 키
>    ```
>
> 10. WebServer 게시
> 11. APK 배포

---

<h3 align="center">iOS 미개발</h3>
