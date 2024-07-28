## [2.3.5](https://github.com/no-slopes/sprite-animations/compare/v2.3.4...v2.3.5) (2024-07-28)


### Bug Fixes

* Eliminate multiple GC.Alloc caused by Enum.Equals ([c107a1a](https://github.com/no-slopes/sprite-animations/commit/c107a1aa9493518cd8e56f04b5746ba25e4ed4c0))

## [2.3.4](https://github.com/no-slopes/sprite-animations/compare/v2.3.3...v2.3.4) (2024-04-02)


### Bug Fixes

* Fix bug [#4](https://github.com/no-slopes/sprite-animations/issues/4) ([77b88dd](https://github.com/no-slopes/sprite-animations/commit/77b88dd6135571596d7751c734526633bc555968))

## [2.3.3](https://github.com/no-slopes/sprite-animations/compare/v2.3.2...v2.3.3) (2023-12-04)


### Bug Fixes

* Types issue caused by v2.3.2 changes. ([cdb3c8c](https://github.com/no-slopes/sprite-animations/commit/cdb3c8c25c66a78dd6a7faf59883c59853c92f76))

## [2.3.2](https://github.com/no-slopes/sprite-animations/compare/v2.3.1...v2.3.2) (2023-12-04)


### Bug Fixes

* Impeditive bug when building ([7fa3353](https://github.com/no-slopes/sprite-animations/commit/7fa33538cb7fc30ec529ecc7a1d8dff9b3a7a2ce))

## [2.3.1](https://github.com/no-slopes/sprite-animations/compare/v2.3.0...v2.3.1) (2023-11-06)


### Bug Fixes

* samples and unwanted logs ([579abe5](https://github.com/no-slopes/sprite-animations/commit/579abe5a5749d44c8b80612bd86e1cec400e82c5))

# [2.3.0](https://github.com/no-slopes/sprite-animations/compare/v2.2.0...v2.3.0) (2023-11-06)


### Features

* **CompositeAnimator:** composite animator works. ([80f8ea5](https://github.com/no-slopes/sprite-animations/commit/80f8ea515a82930e9fab279af0c4b730b422147f))

# [2.2.0](https://github.com/no-slopes/sprite-animations/compare/v2.1.0...v2.2.0) (2023-11-06)


### Features

* **AnimationsManager:** Composite Animation. ([ebaa09f](https://github.com/no-slopes/sprite-animations/commit/ebaa09f64b787fc0b0264c0dba6a4b4b694a85b9))

# [2.1.0](https://github.com/no-slopes/sprite-animations/compare/v2.0.2...v2.1.0) (2023-11-01)


### Features

* **animations:** Templates and OnDemand ([048dc67](https://github.com/no-slopes/sprite-animations/commit/048dc6704f6e94dfafb18db28839297fd7bfd8ed))
* **SingleCycle:** Single Cycle as Template ([1dd82d6](https://github.com/no-slopes/sprite-animations/commit/1dd82d616902a5b119e35688af29c67ffbff7821))
* **SpriteAnimation:** open manager button ([3647720](https://github.com/no-slopes/sprite-animations/commit/3647720413ac8ffd016de95fe2cd4ff54806b11c))
* **SpriteAnimation:** single animation manager ([f37cfc4](https://github.com/no-slopes/sprite-animations/commit/f37cfc464c343eb400bdfb0f746c7d6b129344c9))

## [2.0.2](https://github.com/no-slopes/sprite-animations/compare/v2.0.1...v2.0.2) (2023-10-31)


### Bug Fixes

* animations manager losing reference ([8c0ddfd](https://github.com/no-slopes/sprite-animations/commit/8c0ddfddb06039c936583bab456dde9107ecd271))

## [2.0.1](https://github.com/no-slopes/sprite-animations/compare/v2.0.0...v2.0.1) (2023-10-28)


### Bug Fixes

* **CR:** Correcting samples folder problem with continuous ([75a04e5](https://github.com/no-slopes/sprite-animations/commit/75a04e554075738da7fe507637caf6c89424fe14))

# [2.0.0](https://github.com/no-slopes/sprite-animations/compare/v1.2.5...v2.0.0) (2023-10-28)

### We now have Combo Animations! 

### Bug Fixes

* **AnimationsManager:** the combo animation window ([1870992](https://github.com/no-slopes/sprite-animations/commit/187099218cdc2458b6751e877b9298763e60f153))

### Features

* **animation:** created the combo animation structure. ([df4c403](https://github.com/no-slopes/sprite-animations/commit/df4c4037e327982f9ccfd9a5f6cdc069eebe5167))
* **AnimationManager:** combo animation manager window ([1cada17](https://github.com/no-slopes/sprite-animations/commit/1cada174d2d3105554a476df9748c92170f60cf7))
* **AnimationsManager:** caches the last location used ([8ba4d1c](https://github.com/no-slopes/sprite-animations/commit/8ba4d1c275dd91ca675e3b2b5652ed151c822dac))
* **performer:** combo animator is now working. ([ac409ee](https://github.com/no-slopes/sprite-animations/commit/ac409eed989a409f6c75e89b902669c7e0ccfd8c))


### BREAKING CHANGES

- The animations made in previous versions will not work due to a change in how cycles calculate their own stuff (frames, duration and etc). For that they need their animation to be injected when they are created and previous animation did not do that.
- SingleAnimation is now called SingleCycleAnimation in order to stablish a more clear understanding of how it works upon reading its name.

* **performer:** The simple animations are now called SingleCycle, as well
as all features related. Animations previously made will need to
be recreated as the Animations Manager now injects the
animations into its cycles in order to allow the cycle to
perform its own frame calculations.

## [1.2.5](https://github.com/no-slopes/sprite-animations/compare/v1.2.4...v1.2.5) (2023-10-26)


### Bug Fixes

* **animator:** remove Debug.Log when playing animations. ([9c75a59](https://github.com/no-slopes/sprite-animations/commit/9c75a5951a649e002fe1612cf9eb6a5e3210bf1a))

## [1.2.4](https://github.com/no-slopes/sprite-animations/compare/v1.2.3...v1.2.4) (2023-10-26)


### Bug Fixes

* **Performers:** Fixed a bug related to accessing ([995d225](https://github.com/no-slopes/sprite-animations/commit/995d225ab54bd9f89e3cbbbbdc68f0e5b3163b2d))


### Performance Improvements

* **Performers:** now the cycle is responsible for ([f80e8cb](https://github.com/no-slopes/sprite-animations/commit/f80e8cbf364af70b708bfc8a5d5d8998f5cb2a30))

## [1.2.3](https://github.com/no-slopes/sprite-animations/compare/v1.2.2...v1.2.3) (2023-10-26)


### Bug Fixes

* **AnimationType:** add combo animation type ([062fcc0](https://github.com/no-slopes/sprite-animations/commit/062fcc0c0115f6f753b74f280184e93515705c3e))

## [1.2.2](https://github.com/no-slopes/sprite-animations/compare/v1.2.1...v1.2.2) (2023-10-26)


### Bug Fixes

* **Animator:** SpriteAnimator no longer requires a ([50d6764](https://github.com/no-slopes/sprite-animations/commit/50d6764e9648f357ffc2fce59a5784a4a4727121))

## [1.2.1](https://github.com/no-slopes/sprite-animations/compare/v1.2.0...v1.2.1) (2023-10-26)


### Bug Fixes

* **metafiles:** add .meta removal for Samples~ folder ([2f016fe](https://github.com/no-slopes/sprite-animations/commit/2f016fe9798e949593c73828b9ef0a9952388758))

# [1.2.0](https://github.com/no-slopes/sprite-animations/compare/v1.1.0...v1.2.0) (2023-10-26)


### Features

* StateMachine (CR testing purrposes. sorry guys) ([1995808](https://github.com/no-slopes/sprite-animations/commit/1995808e49c988ca31f3baee23ef434d23949376))

# [1.1.0](https://github.com/no-slopes/sprite-animations/compare/v1.0.2...v1.1.0) (2023-10-26)


### Features

* Reorganizing folder structure and CR ([c2f1c7a](https://github.com/no-slopes/sprite-animations/commit/c2f1c7a446e8a27f9229032fa8d02a40816de57b))

### Changelog

All notable changes to this project will be documented in this file. Dates are displayed in UTC.

Generated by [`auto-changelog`](https://github.com/CookPete/auto-changelog).

#### [v1.1.3-beta](https://github.com/no-slopes/sprite-animations/compare/v1.1.2-beta...v1.1.3-beta)

- Restructured. Trying to create the Symlink [`3aed11c`](https://github.com/no-slopes/sprite-animations/commit/3aed11c21a29b504e1c47a6d99f15d55a82737d0)
- Symlink created [`def7938`](https://github.com/no-slopes/sprite-animations/commit/def793800c501bf58a6d4439e45f647b3f6c603c)
- Showcase at docs [`958cb0e`](https://github.com/no-slopes/sprite-animations/commit/958cb0e2285552bfa119bcc584dc6a5265483235)

#### [v1.1.2-beta](https://github.com/no-slopes/sprite-animations/compare/v1.1.1-beta...v1.1.2-beta)

> 24 October 2023

- Documentation and package json [`854cd24`](https://github.com/no-slopes/sprite-animations/commit/854cd24e6cd17933efcde1c26bad54a76abc64ac)
- Adding some badges to docs [`0413b4d`](https://github.com/no-slopes/sprite-animations/commit/0413b4dd80079523b3da278537c56d6199d231da)
- Index [`d3f5323`](https://github.com/no-slopes/sprite-animations/commit/d3f5323d95cee761d0224a2cd97a2faa0ca2aa3a)

#### [v1.1.1-beta](https://github.com/no-slopes/sprite-animations/compare/v1.1.0-preview.5...v1.1.1-beta)

> 19 October 2023

- Documenting for release [`80e060b`](https://github.com/no-slopes/sprite-animations/commit/80e060b93b08a9e201b592bdfc9055b3e04b7c8b)
- Merge tag 'v1.1.0-beta' into develop [`bed4596`](https://github.com/no-slopes/sprite-animations/commit/bed4596a6d5a4ec57a7655f7e6f0dee11328db18)
- Merge tag 'v1.1.0' into develop [`f833b0a`](https://github.com/no-slopes/sprite-animations/commit/f833b0a41364869a95beb1ace5762683c2c4ca8d)

#### [v1.1.0-preview.5](https://github.com/no-slopes/sprite-animations/compare/v1.1.0-preview.4...v1.1.0-preview.5)

> 19 October 2023

- Merge tag 'v1.1.0-preview.4' into develop [`5fdd91c`](https://github.com/no-slopes/sprite-animations/commit/5fdd91cd0edf5b1f1d0d10119f0e450b6019432b)

#### [v1.1.0-preview.4](https://github.com/no-slopes/sprite-animations/compare/v1.1.0-preview.2...v1.1.0-preview.4)

> 19 October 2023

- Docs [`8d5e69b`](https://github.com/no-slopes/sprite-animations/commit/8d5e69b1f6c3baa33e6395ba7d441bacf10f255b)
- Samples folder path [`91c812e`](https://github.com/no-slopes/sprite-animations/commit/91c812ea7570e0491d7bd3466d8911959a0d8f14)
- Merge tag 'vv1.1.0-preview.3' into develop [`df949b2`](https://github.com/no-slopes/sprite-animations/commit/df949b2e31b782d4352798544d74bb2b1d437c28)

#### [v1.1.0-preview.2](https://github.com/no-slopes/sprite-animations/compare/v1.1.0-preview.1...v1.1.0-preview.2)

> 19 October 2023

- Documenting release [`925a3ad`](https://github.com/no-slopes/sprite-animations/commit/925a3ad2abce3b9e6496fd9803fb4e9f1b91bbd9)
- Changed package description [`c12485d`](https://github.com/no-slopes/sprite-animations/commit/c12485d81a8c820d3d84ad99a249c50ee1562cd8)
- Correcting samples [`7ebe3d8`](https://github.com/no-slopes/sprite-animations/commit/7ebe3d8c0221022aea36b8978743207c90e94f91)

#### [v1.1.0-preview.1](https://github.com/no-slopes/sprite-animations/compare/v1.1.0-preview.0...v1.1.0-preview.1)

> 19 October 2023

- Releasing [`794847d`](https://github.com/no-slopes/sprite-animations/commit/794847d6614976fe687d59af9d047e71daef5d19)
- Changelog to docs [`98c07b6`](https://github.com/no-slopes/sprite-animations/commit/98c07b646d46f7ababf279dbf156746ebd431e4d)
- chore: release v1.1.0-preview.0 [`1b6ee98`](https://github.com/no-slopes/sprite-animations/commit/1b6ee98b5434ce6ed9f031ea6ce048153a9d8711)

#### [v1.1.0-preview.0](https://github.com/no-slopes/sprite-animations/compare/v1.1.0-beta...v1.1.0-preview.0)

> 19 October 2023

#### [v1.1.0-beta](https://github.com/no-slopes/sprite-animations/compare/v1.0.2...v1.1.0-beta)

> 19 October 2023

- Merge tag 'v1.1.0' into develop [`f833b0a`](https://github.com/no-slopes/sprite-animations/commit/f833b0a41364869a95beb1ace5762683c2c4ca8d)
- Documenting for v1.1.0-beat [`70bba3c`](https://github.com/no-slopes/sprite-animations/commit/70bba3c54c83f3416dcffef92c5016e47938325a)
- Merge tag 'v1.1.0-preview.5' into develop [`e250228`](https://github.com/no-slopes/sprite-animations/commit/e25022865fd4f06bb05977133642af62475f4847)

#### [v1.0.2](https://github.com/no-slopes/sprite-animations/compare/v1.0.1...v1.0.2)

> 19 October 2023

- Adding meta tags to head element [`69650c0`](https://github.com/no-slopes/sprite-animations/commit/69650c0e7ebae51587b969a3551359099f20aebf)
- Adding samples [`68c99a7`](https://github.com/no-slopes/sprite-animations/commit/68c99a7db35dc65fcceede3c0049e0661d2414b8)
- docs metas [`d4cc78b`](https://github.com/no-slopes/sprite-animations/commit/d4cc78b45e95d06c484f304abb675ee1a8193808)

#### [v1.0.1](https://github.com/no-slopes/sprite-animations/compare/v1.0.0...v1.0.1)

> 16 October 2023

- Documenting before release [`645c877`](https://github.com/no-slopes/sprite-animations/commit/645c877682588a46268771cbc2174da710d2b6fb)
- Documentation metas [`224af13`](https://github.com/no-slopes/sprite-animations/commit/224af13f37137eff27a39d3574ddde62e7282669)
- Ready to release [`4881528`](https://github.com/no-slopes/sprite-animations/commit/4881528de8c8d92e858f2d2b5bf3f1bc697506e4)

### [v1.0.0](https://github.com/no-slopes/sprite-animations/compare/v0.2.9...v1.0.0)

> 16 October 2023

- Documenting [`e9d335d`](https://github.com/no-slopes/sprite-animations/commit/e9d335dd1d0f0081fdf692065343d8baae3f253a)
- Documenting [`3629aa3`](https://github.com/no-slopes/sprite-animations/commit/3629aa3e888cf3c86827b62d4b99b3261e5bf288)
- The sprite animator is documented [`5e1ec0c`](https://github.com/no-slopes/sprite-animations/commit/5e1ec0cfc3de831de7d9bc85f9d307672bdeae4a)

#### [v0.2.9](https://github.com/no-slopes/sprite-animations/compare/v0.2.8...v0.2.9)

> 15 October 2023

- Restoring docs [`da53eb7`](https://github.com/no-slopes/sprite-animations/commit/da53eb75678e6023bbfe9cc3234705de49dc511b)
- chore: release v0.2.9 [`d4f5327`](https://github.com/no-slopes/sprite-animations/commit/d4f532734080a3c949542deb0639db69df34a109)

#### [v0.2.8](https://github.com/no-slopes/sprite-animations/compare/v0.2.7...v0.2.8)

> 15 October 2023

- Last documentation style touches [`9fb9078`](https://github.com/no-slopes/sprite-animations/commit/9fb9078097ad8354a399cc98a97588acbd293d3a)
- Removed docs [`38116c2`](https://github.com/no-slopes/sprite-animations/commit/38116c2774dd0cd717ff76d7927b2846dce46684)
- Generated new documentation [`5760eba`](https://github.com/no-slopes/sprite-animations/commit/5760ebac32e05781f700b1d0db5af3b467c5656a)

#### [v0.2.7](https://github.com/no-slopes/sprite-animations/compare/v0.2.6...v0.2.7)

> 15 October 2023

- Ready to start documenting [`7b52a0c`](https://github.com/no-slopes/sprite-animations/commit/7b52a0c0e49b4692979e26794a5ccfe600237142)
- chore: release v0.2.7 [`a3e3e1c`](https://github.com/no-slopes/sprite-animations/commit/a3e3e1c99b6498496458a06a80d9835eb1849f53)
- Last touch to release flow [`f1e6e81`](https://github.com/no-slopes/sprite-animations/commit/f1e6e814d9753efa3e86e93cdf5263d56acfe9e9)

#### [v0.2.6](https://github.com/no-slopes/sprite-animations/compare/v0.2.5...v0.2.6)

> 15 October 2023

- Extracting docfx folder [`a2550ef`](https://github.com/no-slopes/sprite-animations/commit/a2550ef15a4821b62af30f11c3bb8d5e02fc59ae)
- chore: release v0.2.6 [`21963a9`](https://github.com/no-slopes/sprite-animations/commit/21963a9e972a8490002201a4c5af93e829d09f1a)
- Still working on docs [`1dbfb73`](https://github.com/no-slopes/sprite-animations/commit/1dbfb7330e7b7f3183b084874c7142b6cd796d17)

#### [v0.2.5](https://github.com/no-slopes/sprite-animations/compare/v0.2.4...v0.2.5)

> 14 October 2023

- Adding darkfx theme to docfx [`53cef6e`](https://github.com/no-slopes/sprite-animations/commit/53cef6e9c7d58013f31fa519087e7cade3af2057)
- Testing documentation changelog [`e014b2f`](https://github.com/no-slopes/sprite-animations/commit/e014b2ffcef2dfe3a4c5652db5be0a83e99ba9e5)
- chore: release v0.2.5 [`96d7cc3`](https://github.com/no-slopes/sprite-animations/commit/96d7cc3edacbdb488597f8483daf25ebdf0d2b6b)

#### [v0.2.4](https://github.com/no-slopes/sprite-animations/compare/v0.2.3...v0.2.4)

> 14 October 2023

- chore: release v0.2.4 [`3153c59`](https://github.com/no-slopes/sprite-animations/commit/3153c59243eb37d3861036c0b1c9eb5c6e2fe91d)
- Release flow [`53ce0de`](https://github.com/no-slopes/sprite-animations/commit/53ce0de214d3646cce744c228553bafeb2121107)
- Testing release flow [`9b63cf7`](https://github.com/no-slopes/sprite-animations/commit/9b63cf7e7f3c46afed53ca5197fc18e311af416e)

#### [v0.2.3](https://github.com/no-slopes/sprite-animations/compare/v0.2.2...v0.2.3)

> 14 October 2023

- Docs Setup [`47a2a60`](https://github.com/no-slopes/sprite-animations/commit/47a2a60f0406bbffca24251bf994cbd7a6c19ac5)
- Docs first commit [`2951894`](https://github.com/no-slopes/sprite-animations/commit/29518946d6847da1787bf9dd322c476194bb93a4)
- Generating Docs [`e1f80e6`](https://github.com/no-slopes/sprite-animations/commit/e1f80e6a3b4ce100e82a97d86eb59fac5438998c)

#### [v0.2.2](https://github.com/no-slopes/sprite-animations/compare/v0.2.1...v0.2.2)

> 13 October 2023

- Windrose animation view layout changed to columns [`30f58f5`](https://github.com/no-slopes/sprite-animations/commit/30f58f5e32ef7a33ef453f1aec17d92349902e07)
- Fixed bug on animations list [`71e9e4b`](https://github.com/no-slopes/sprite-animations/commit/71e9e4b596fc99ac68ac303bca61a2e09bc211dc)
- chore: release v0.2.2 [`92b3946`](https://github.com/no-slopes/sprite-animations/commit/92b394699b3441032ab160cf4229a3b967bd8fdd)

#### [v0.2.1](https://github.com/no-slopes/sprite-animations/compare/v0.1.1-preview...v0.2.1)

> 12 October 2023

- Preparing release it [`2a9a39b`](https://github.com/no-slopes/sprite-animations/commit/2a9a39b1134541a69ff77c838b5814788b8cc9cb)
- Animation Extractor [`bfffa91`](https://github.com/no-slopes/sprite-animations/commit/bfffa91072b672edfee75231a6e978eef6174558)
- Windrose animation can now be flipped [`29c5593`](https://github.com/no-slopes/sprite-animations/commit/29c55937f00d68dda43b482f3a29ba47492f1e59)

#### [v0.1.1-preview](https://github.com/no-slopes/sprite-animations/compare/v0.1.0-preview...v0.1.1-preview)

> 12 October 2023

- Package Json update [`c3e3ba4`](https://github.com/no-slopes/sprite-animations/commit/c3e3ba45d6174b2de10c8cb6719fd6771f9c06f9)
- Merge tag 'v0.1.0-preview' into develop [`96f671e`](https://github.com/no-slopes/sprite-animations/commit/96f671ebde3138853b8df1ac828eaaa055f033ac)

#### [v0.1.0-preview](https://github.com/no-slopes/sprite-animations/compare/v0.0.1-preview...v0.1.0-preview)

> 11 October 2023

- Windrose Animations [`2c0c619`](https://github.com/no-slopes/sprite-animations/commit/2c0c619f21be47d58e7b988a7c22b78a195ecbf3)
- Sprite Animator Inspector [`121e3e1`](https://github.com/no-slopes/sprite-animations/commit/121e3e17f54034212c5322cf52207b1868e0f438)
- Sprite Animator Play&lt;TAnimator&gt; [`6dd47ea`](https://github.com/no-slopes/sprite-animations/commit/6dd47eacc4faa85147c3858154c42dafdd1f337e)

#### v0.0.1-preview

> 11 October 2023

- Firt Commit [`b70de6a`](https://github.com/no-slopes/sprite-animations/commit/b70de6ad055b7197c468ae67969e6ce72a015940)
- Working [`3446093`](https://github.com/no-slopes/sprite-animations/commit/3446093d2ea343f5d8169d3c04fe8fe44ccba734)
- Windrose almost there [`f88f4fd`](https://github.com/no-slopes/sprite-animations/commit/f88f4fd1a9025168a3ff625a590466f7db197a93)
