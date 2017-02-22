# 구현내용
- 카메라 디바이스에서 캡쳐된 이미지를 분석하여 QRcode BARcode 인식하는 예제
- 캡쳐가 시작되면 1초에 한번씩 이미지를 분석시도하고 성공할때까지 계속하는 단순한 알고리즘.
- QRcode 인식은 꽤 좋은 편이고, BARcode 인식은 성능과 성공률이 낮은편 ㅠㅠ

# 데모
![](http://s24.postimg.org/45xgzlt39/screenshot_51.png)

# 종속성
- windows xp sp3 이상
- 닷넷프레임워크 4.0
    - http://s3.ap-northeast-2.amazonaws.com/coconut-client/dotNetFx40_Full_x86_x64.exe
- 윈도우업데이트 KB2468871
    - http://s3.ap-northeast-2.amazonaws.com/coconut-client/NDP40-KB2468871-v2-x86.exe

# 참고자료
- 코드리더 라이브러리 : 
    - https://zxingnet.codeplex.com
- 웹캠 라이브러리 : 
    - http://www.codeproject.com/Articles/330177/Yet-another-Web-Camera-control
    - https://www.nuget.org/packages/WebEye.Controls.wpf.WebCameraControl