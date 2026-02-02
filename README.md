# Portfolio Project

Unity6 기반 개인 포트폴리오 프로젝트입니다.  
최신 Unity 엔진 기능을 활용하여 구조적 설계, Addressables, 셰이더, UI 시스템 등을 직접 구현한 샘플 프로젝트입니다.

## 프로젝트 개요

- 엔진 : Unity 6000.3.0f1
- 플랫폼 : Android

## 기술 스택
- Unity
- C#
- Addressables
- ScriptableObject
- localization
- DOTween
- Android & iOS
- Git, GitHub
- Jenkins

## 주요 기능

- Addressables 기반 리소스 관리
- ScriptableObject 기반 데이터 설계
- Object Pooling 기반 성능 최적화
- 전투 / 이동 시스템 구현
- UI 팝업 프레임워크 설계
- 무한 스크롤 UI 시스템 구현

### 무한 스크롤 UI 시스템

- 아이템 풀링 기반 재사용 구조
- 중앙 스냅 및 자동 정렬
- 거리 기반 스케일링 효과
- 클릭 시 중앙 이동 기능
- 인덱스 기반 데이터 바인딩

> 관련 코드: /Script/UI/InfiniteScroll/

### UI 팝업 프레임워크

- Stack 기반 팝업 관리
- 우선순위 정렬
- 입력 차단 처리
- 중복 팝업 방지 구조

> 관련 코드: /Script/UI/popup/ , /Script/Manager/UI

### 전투 / 이동 시스템

- Actor 기반 구조 설계
- 상태 기반 행동 처리
- 충돌 판정 분리 구조

> 관련 코드: /Script/Manager/Battle  , /Script/Role , /Script/Core/ActorFactory.cs

### 리소스 관리

- Addressables 비동기 로드
- 참조 카운트 관리
- 메모리 해제 타이밍 제어

> 관련 코드: /Script/Manager/Resource/

## 실행 방법

1. Unity 6000.3.0f1 으로 프로젝트 실행
2. Build Platform 을 Android 로 변경
3. Display 1280x720 Portrait 로 변경
4. 실행