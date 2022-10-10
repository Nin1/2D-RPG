# "Time Walk"
1v1 networked card game with a standalone C# server application and Unity client.

The rules of the game are:
- Players start with 20 life
- Players can play one card from their hand per turn
- Cards are played on the player's timeline according to the card's 'time cost'
- Each turn, the active player:
-- 1. Advances each of their played spells by one
-- 2. Draws a card
-- 3. Plays one card from their hand
-- 4. Resolves all spells in the centre one at a time, in any order

Game architecture:
- Cards loaded from JSON data
- Board state representation and game logic handled in 2D-RPG/Server/CardGameServer/CardGameServer/core/
- Server performs all logic as far as it can, and sends a sequence of 'commands' to the client so it may display what has occurred (2D-RPG/2D RPG/Assets/Script/CardGame/Commands/)
- Client displays what occurred using DOTween for animations.
- Server will wait for the client to send a command back when expecting user input.

Demo game:
https://www.youtube.com/watch?v=KBKzzBSqVI8
