<!-- <p align="right">
  Ler em <a href="README.pt-br.md"> Português do Brasil </a>
</p> -->

[![Compatibility](https://img.shields.io/badge/-2022.3%2B-11191F?logo=Unity&color=5d5d5d)](https://unity.com/releases/editor/archive)
[![Version](https://img.shields.io/npm/v/com.no-slopes.sprite-animations?color=931111&label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.no-slopes.sprite-animations/)
[![Downloads](https://img.shields.io/badge/dynamic/json?color=931111&label=downloads&query=%24.downloads&suffix=%2Fmonth&url=https%3A%2F%2Fpackage.openupm.com%2Fdownloads%2Fpoint%2Flast-month%2Fcom.no-slopes.sprite-animations)](https://openupm.com/packages/com.no-slopes.sprite-animations/)
[![GitHub Repo stars](https://img.shields.io/github/stars/no-slopes/sprite-animations?label=%E2%AD%90&color=931111)](https://github.com/no-slopes/sprite-animations/stargazers)
[![Hits](https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2Fno-slopes%2Fsprite-animations&count_bg=%23931111&title_bg=%23585858&icon=&icon_color=%23943737&title=views&edge_flat=false)](https://hits.seeyoufarm.com)

![Banner](https://no-slopes.github.io/sprite-animations/images/banner.png)

_by No Slopes_

[![Install](https://img.shields.io/badge/%F0%9F%93%81%20Install-7393B3?style=for-the-badge&color=26251f)](https://no-slopes.github.io/sprite-animations/documentation/install.html)  
[![Docs](https://img.shields.io/badge/%F0%9F%93%9A%20Documentation-FFCE00?style=for-the-badge&color=26251f)](https://no-slopes.github.io/sprite-animations)  
[![Kofi](https://img.shields.io/badge/Donate-a73b38?style=for-the-badge&logo=kofi&logoColor=f7404b&color=26251f)](https://ko-fi.com/indiegabo)

A smooth and intuitive way to work with sprite sheets in Unity.

## Showcase

![youtube-showcase_640](https://github.com/no-slopes/sprite-animations/assets/95456621/f99d8ecc-e337-4a99-8cd2-897e16e5d004)

[<img src="https://github.com/no-slopes/sprite-animations/assets/95456621/21942d35-ca0c-420d-b5cc-7a3c60095ba4">](https://www.youtube.com/watch?v=jpoCPpwkFnM)

## Why bother?

- The [Animations Manager](https://no-slopes.github.io/sprite-animations/documentation/animations-manager/index.html) window provides qualitity of life when managing your animations and frames.
- Better perfomance than the Unity Animator since it only handles changing sprites at the SpriteRenderer and nothing else.
- Coding made easy.
- Perfect for state driven (FSM) characters.

Checkout [how it works](https://no-slopes.github.io/sprite-animations/documentation/how-it-works.html) and give it a try!

## Key features:

##### Sprite Animator

The [Sprite Animator](https://no-slopes.github.io/sprite-animations/documentation/sprite-animator/index.html) allows to play animations
by simply changing the sprites of a SpriteRenderer.

##### Animations Manager

The [Animations Manager](https://no-slopes.github.io/sprite-animations/documentation/animations-manager/index.html) is an Editor window that gives
you quality of life when editing your animations.

##### Animations

- [Single Cycle Animations](https://no-slopes.github.io/sprite-animations/documentation/animations/single-cycle-animation.html) are the most basic type of animation. Just define [Frame Cycles](https://no-slopes.github.io/sprite-animations/documentation/animations-manager/index.html#frame-cycle) and you are good to go.
- [Windrose Animations](https://no-slopes.github.io/sprite-animations/documentation/animations/windrose-animation.html) are meant to help those working with top-dow games. Allows you to create one [Frame Cycle](https://no-slopes.github.io/sprite-animations/documentation/animations-manager/index.html#frame-cycle) for each of the cardinal
  positions and then it is easy to just change the direction based on the user's input.
- [Combo Animations](https://no-slopes.github.io/sprite-animations/documentation/animations/combo-animation.html) are a collection of [Frame Cycles](https://no-slopes.github.io/sprite-animations/documentation/animations-manager/index.html#frame-cycle) that can have their execution chained. It plays one cycle, waits for user input, and then
  plays the next one.

## Beta Version

This tool should be carefully tested before integrated in a project that has commercial intentions. Although I really trust
its capacity, it is still too young to be considered stable.

## Installing

https://github.com/no-slopes/sprite-animations/assets/95456621/e1ebc0ab-471e-437a-821c-7180749b2e7d

Detailed instructions [here](https://no-slopes.github.io/sprite-animations/documentation/install.html)

## Unity tested versions

[![Compatibility](https://img.shields.io/badge/-2022.3%2B-11191F?logo=Unity&color=5d5d5d)](https://unity.com/releases/editor/archive)

## About No Slopes

We are a small group of game devs / artists building an Indie Game Studio and crafting some
tools along the way.

## The author

![portraitzinho-fundo-roxo-rounded](https://github.com/no-slopes/sprite-animations/assets/95456621/2b6523f8-71b4-424c-ae61-5c5d1a3b639e)

My name is Gabriel, ppl know me by [Gabo](https://github.com/indiegabo) and you can find me as @indiegabo or @indiegabodev in most social networks.

If you want to buy me a coffee that will be much appreciated.

[![Kofi](https://img.shields.io/badge/Donate-a73b38?style=for-the-badge&logo=kofi&logoColor=f7404b&color=26251f)](https://ko-fi.com/indiegabo)

## Special thanks and credits:

- [DanniBoy](https://www.linkedin.com/in/daniel-souz/) for being my No Slopes associate and desining the logo and icons for the project
- [Cammin](https://github.com/Cammin), creator of [LDtk to Unity](https://github.com/Cammin/LDtkToUnity), for being such a friendly person and giving me directions on how to craft this docs page. BTW if you do not know [LDtk to Unity](https://github.com/Cammin/LDtkToUnity) yet, you should really give it a go if you work with 2D levels in unity. The quality of life this tool provide is insane!
- [Gabriel Bigardi](https://github.com/GabrielBigardi) for introducing me the concept of changing sprites of a SpriteRenderer directly instead of having Unity's animator to do so.

##### Assets used during showcase

- [EVil Wizard 2](https://luizmelo.itch.io/evil-wizard-2)
- [Medieval Warrior Pack 2](https://luizmelo.itch.io/medieval-warrior-pack-2)
- [8-direction Top Down Character](https://gamekrazzy.itch.io/8-direction-top-down-character)
