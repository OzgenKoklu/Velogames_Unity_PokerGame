# Velogames - Unity - Poker Game

Made with Unity 2022.3.0f1 LTS 

----------
<!-- TABLE OF SECTIONS -->
  # Sections
  <ol>
	  <li><a href="#Introduction">Introduction</a></li>
	  <li><a href="#DevLog">Dev Log</a></li>
	  <li><a href="#KnownBugs">Known Bugs</a></li>
	  <li><a href="#Videos">Videos</a></li>
	  <li><a href="#TechnicalDetails">Technical Details</a></li>
	  <li><a href="#Acknowledgements">Acknowledgements</a></li>
  </ol>

----------

<!-- INTRODUCTION -->
## Introduction

A task project assigned by Velo Games.

A slot game bla bla.

More info in pdf in the root folder.

Zip for the newest PC Built  "TBA".

## Team Members: 

Özgen Köklü : https://github.com/OzgenKoklu

Ezgi Keserci : https://github.com/ezgiksrci

----------

<!-- DevLog -->
## DevLog

GDD for the game: 

https://docs.google.com/document/d/1RrYZFcERAm4OzjKxE57lmc0hMOfWjcYncdkV3LErDXQ/edit?usp=sharing

## Notes on PokerHandEvaluator.cs

EvaluateAndFindWinner takes a list of list of CardSO, which are a list of player hands (hole cards), combined with community cards.

Loop through the players and EvaluateHand() function calculates which hand type is present and also with the best card in that pair.

For example, it knows if a player has a two pair, and the best pair is a pair of Jokers. Or if player has a full house (a three of a kind and a pair), it knows the rank of the three pair.

By this stage algorithm does not take track of the "kicker cards" to unnecessary computation.

Then, if the best hand is beaten in EvaluateAndFindWinner()'s for loop, it resets the "Potential Winners" list, and adds new potential winners to that list only if the new player has the same hand type.

Later, if the potential winners list lenght is bigger than 1, it does another check with the rank of that specific hand type, and eliminated the lower ranking hands.

For example, 3 hands with One Pair, 4, 6, 6 for their pair ranks respectively, hand with pair that has 4 rank is eliminated. 

After this stage, tiebreaker should work, and for that tie breaker, we will have to check the "kicker cards" that these potential winners hold.

That algorithm is not present at this moment. There is a filler section, but it does not work.

NOTES

NOTES

NOTES

NOTES

NOTES

# Day 0 - LOREM IPSUN

<img src="Media/????.PNG" width="900"> 

----------
<!-- KnownBugs -->
## Known Bugs to adress for the ??.03.2024 - Final Commit: 

1) 


----------

<!-- Videos -->
## Videos

??.03.2024 - Working in editor: 

NOTES

[![Youtube Link](https://img.youtube.com/vi/YOUTUBELINK/0.jpg)](https://youtu.be/YOUTUBELINK)

----------

<!-- TechnicalDetails -->
## TechnicalDetails

-

-

-

----------

<!-- Acknowledgements -->
## Acknowledgements

A non profit project. 

## In Game Assets:

Main Background:
AI image generation. Dall-E - GPT4

Playing Cards: 
Casino Pack 1001com
https://opengameart.org/content/casino-pack

Player Avatars - Pexels Stock images: (CC0)
Photo by Andrea Piacquadio: https://www.pexels.com/photo/closeup-photo-of-woman-with-brown-coat-and-gray-top-733872/
Photo by Andrea Piacquadio: https://www.pexels.com/photo/woman-in-collared-shirt-774909/
Photo by Thgusstavo Santana: https://www.pexels.com/photo/man-with-cigarette-in-mouth-1933873/
Photo by Simon Robben: https://www.pexels.com/photo/man-in-brown-polo-shirt-614810/
Photo by Stefan Stefancik: https://www.pexels.com/photo/man-on-gray-shirt-portrait-91227/
Photo by Daniel Xavier: https://www.pexels.com/photo/woman-wearing-black-eyeglasses-1239291/
Photo by Andrea Piacquadio: https://www.pexels.com/photo/man-wearing-red-sweatshirt-and-black-pants-leaning-on-the-wall-845434/
Photo by Tuấn Kiệt Jr.: https://www.pexels.com/photo/woman-posing-for-photo-shoot-1391498/


## Sounds Assets: Unused

## UI Assets: 

