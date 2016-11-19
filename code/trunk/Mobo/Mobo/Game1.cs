/**
* Author: Christopher Cola
* Created on 17/10/2015
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace Mobo
{
    /// This is the main type for your game ///
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        // The static spritebatch is used to draw stuff that doesn't move relative to the player (i.e. UI)
        // The camera spritebatch draws stuff relative to the position of the player (i.e. background)
        SpriteBatch staticSpriteBatch;
        SpriteBatch cameraSpriteBatch;

        MainMenu mainMenu = new MainMenu();
        Offline offline = new Offline();
        Online online = new Online();
        Help help = new Help();
        Settings settings = new Settings();
        StationGenerator stationGenerator = new StationGenerator();

        MessageList msgList = new MessageList();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = false; //Hide system cursor
            Content.RootDirectory = "Content"; // Set root content directory
            Window.Title = "MOBO"; // Set window title

            // Load in settings from file
            SettingsManager.Initialize();

            // Set resolution to that specified by settings
            graphics.PreferredBackBufferWidth = SettingsManager.getResolutionWidth();
            graphics.PreferredBackBufferHeight = SettingsManager.getResolutionHeight();
        }

        protected override void Initialize()
        {
            // Initialise viewport variables
            ScreenManager.screenCenter = new Vector2(SettingsManager.getResolutionWidth() / 2, SettingsManager.getResolutionHeight() / 2);

            // Set the state of the game and the cursor to their initial values
            ScreenManager.gameState = GameState.MainMenu;
            ScreenManager.cursorState = CursorState.Pointer;

            // Pass the messageList to ScreenManager so that it can be added to elsewhere
            ScreenManager.messageList = msgList;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Initialise the spriteBatches
            staticSpriteBatch = new SpriteBatch(GraphicsDevice);
            cameraSpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all the assets into memory (texures, sound etc.)
            new ContentStore(Content);

            // Initialise all the GameStates, loading their content ready for displaying
            mainMenu.Initialize();
            offline.Initialize();
            online.Initialize();
            help.Initialize();
            settings.Initialize();
            stationGenerator.Initialize();

            // Load debug tex
            ContentStore.debug = new Texture2D(GraphicsDevice, 1, 1);
            ContentStore.debug.SetData(new Color[] { Color.White });
        }

        protected override void UnloadContent()
        {
            // Nothing to unload as of yet
        }

        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (IsActive)
            {
                // Mouse positioning and state
                ScreenManager.mouse = Mouse.GetState();
                ScreenManager.cursorPos = ScreenManager.mouse.Position.ToVector2();

                // Keyboard state
                ScreenManager.keyboard = Keyboard.GetState(); 
            }

            // Only update the class that corresponds to the gamestate
            switch (ScreenManager.gameState)
            {
                case GameState.MainMenu: mainMenu.Update(); break;
                case GameState.Offline: offline.Update(); break;
                case GameState.Online: online.Update(); break;
                case GameState.Help: help.Update(); break;
                case GameState.Settings: settings.Update(); break;
                case GameState.Exiting: this.Exit(); break;
                case GameState.StationGenerator: stationGenerator.Update(); break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Static elements like the UI
            staticSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);

            // Everything else, backgrounds, players etc.
            cameraSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camera2D.getTransformation(GraphicsDevice));

            // Only draw the class related to the current gamestate
            switch (ScreenManager.gameState)
            {
                case GameState.MainMenu: mainMenu.Draw(staticSpriteBatch); break;
                case GameState.Offline: offline.Draw(staticSpriteBatch, cameraSpriteBatch); break;
                case GameState.Online: online.Draw(staticSpriteBatch, cameraSpriteBatch); break;
                case GameState.Help: help.Draw(staticSpriteBatch); break;
                case GameState.Settings: settings.Draw(staticSpriteBatch); break;
                case GameState.StationGenerator: stationGenerator.Draw(staticSpriteBatch); break;
            }

            // Draw the cursor based on the contextual cursor state
            switch (ScreenManager.cursorState)
            {
                case CursorState.Pointer: staticSpriteBatch.Draw(ContentStore.cursor_pointer, ScreenManager.cursorPos, Color.White); break;
                case CursorState.Hand: staticSpriteBatch.Draw(ContentStore.cursor_hand, ScreenManager.cursorPos, Color.White); break;
                case CursorState.Crosshair: staticSpriteBatch.Draw(ContentStore.cursor_crosshair, ScreenManager.cursorPos, Color.White); break;
            }

            // Draw the message pane
            msgList.Draw(staticSpriteBatch);

            staticSpriteBatch.End();
            cameraSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
