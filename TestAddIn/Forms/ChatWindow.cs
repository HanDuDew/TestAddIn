using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvAddIn.Services;
using InvAddIn.Common;

namespace InvAddIn.Forms
{
    public partial class ChatWindow : UserControl
    {
        private TextBox chatDisplayTextBox;
        private TextBox chatInputTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button clearButton;
        private Label statusLabel;
        private AIChatService _aiChatService;
        private List<string> _chatHistory;

        public ChatWindow()
        {
            _chatHistory = new List<string>();
            InitializeComponent();
            InitializeAIService();
        }

        private void InitializeComponent()
        {
            this.chatDisplayTextBox = new System.Windows.Forms.TextBox();
            this.chatInputTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chatDisplayTextBox
            // 
            this.chatDisplayTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatDisplayTextBox.BackColor = System.Drawing.Color.White;
            this.chatDisplayTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatDisplayTextBox.Location = new System.Drawing.Point(10, 10);
            this.chatDisplayTextBox.Multiline = true;
            this.chatDisplayTextBox.Name = "chatDisplayTextBox";
            this.chatDisplayTextBox.ReadOnly = true;
            this.chatDisplayTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatDisplayTextBox.Size = new System.Drawing.Size(644, 403);
            this.chatDisplayTextBox.TabIndex = 0;
            this.chatDisplayTextBox.Text = "AI Chat Assistant - Ask me anything about your Inventor project!\r\n\r\n";
            this.chatDisplayTextBox.TextChanged += new System.EventHandler(this.chatDisplayTextBox_TextChanged);
            // 
            // chatInputTextBox
            // 
            this.chatInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatInputTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatInputTextBox.Location = new System.Drawing.Point(10, 443);
            this.chatInputTextBox.Multiline = true;
            this.chatInputTextBox.Name = "chatInputTextBox";
            this.chatInputTextBox.Size = new System.Drawing.Size(524, 40);
            this.chatInputTextBox.TabIndex = 1;
            this.chatInputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChatInputTextBox_KeyDown);
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.BackColor = System.Drawing.Color.White;
            this.sendButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.ForeColor = System.Drawing.Color.Black;
            this.sendButton.Location = new System.Drawing.Point(548, 443);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(106, 40);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = false;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.BackColor = System.Drawing.Color.White;
            this.clearButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.ForeColor = System.Drawing.Color.Black;
            this.clearButton.Location = new System.Drawing.Point(548, 494);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(106, 40);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.Gray;
            this.statusLabel.Location = new System.Drawing.Point(5, 499);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(72, 30);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Ready";
            // 
            // ChatWindow
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatInputTextBox);
            this.Controls.Add(this.chatDisplayTextBox);
            this.Name = "ChatWindow";
            this.Size = new System.Drawing.Size(664, 543);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void InitializeAIService()
        {
            // Try to load saved API key
            string savedApiKey = ConfigurationHelper.LoadApiKey();
            
            if (!string.IsNullOrEmpty(savedApiKey))
            {
                try
                {
                    _aiChatService = new AIChatService(savedApiKey);
                    statusLabel.Text = "AI Ready";
                    statusLabel.ForeColor = Color.Green;
                    AppendToChatDisplay("AI service connected using saved API key!\r\n\r\n");
                }
                catch (Exception ex)
                {
                    statusLabel.Text = "AI Unavailable";
                    statusLabel.ForeColor = Color.Red;
                    AppendToChatDisplay($"Error initializing AI service with saved key: {ex.Message}\r\n\r\n");
                }
            }
            else
            {
                statusLabel.Text = "API Key Required";
                statusLabel.ForeColor = Color.Orange;
                AppendToChatDisplay("Please configure your OpenAI API key via Settings ¡ú Configure API Key.\r\n\r\n");
            }
        }

        public void SetApiKey(string apiKey)
        {
            try
            {
                _aiChatService = new AIChatService(apiKey);
                statusLabel.Text = "AI Ready";
                statusLabel.ForeColor = Color.Green;
                AppendToChatDisplay("AI service connected successfully!\r\n");
                
                // Save the API key for future use
                ConfigurationHelper.SaveApiKey(apiKey);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "AI Unavailable";
                statusLabel.ForeColor = Color.Red;
                AppendToChatDisplay($"Error connecting to AI service: {ex.Message}\r\n");
            }
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            await SendMessageAsync();
        }

        private async void ChatInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.Handled = true;
                await SendMessageAsync();
            }
        }

        private async Task SendMessageAsync()
        {
            string userMessage = chatInputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage))
                return;

            if (_aiChatService == null)
            {
                AppendToChatDisplay("AI service not available. Please check your API key.\r\n");
                return;
            }

            // Clear input and show user message
            chatInputTextBox.Clear();
            AppendToChatDisplay($"User: {userMessage}\r\n");
            _chatHistory.Add($"User: {userMessage}");

            // Show thinking status
            statusLabel.Text = "AI is thinking...";
            statusLabel.ForeColor = Color.Orange;
            sendButton.Enabled = false;

            try
            {
                // Get AI response with context
                string response = await _aiChatService.SendMessageWithContextAsync(userMessage, _chatHistory.ToArray());
                
                AppendToChatDisplay($"AI: {response}\r\n\r\n");
                _chatHistory.Add($"AI: {response}");

                statusLabel.Text = "Ready";
                statusLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                AppendToChatDisplay($"Error: {ex.Message}\r\n\r\n");
                statusLabel.Text = "Error occurred";
                statusLabel.ForeColor = Color.Red;
            }
            finally
            {
                sendButton.Enabled = true;
                chatInputTextBox.Focus();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            chatDisplayTextBox.Clear();
            _chatHistory.Clear();
            chatDisplayTextBox.Text = "AI Chat Assistant - Ask me anything about your Inventor project!\r\n\r\n";
            statusLabel.Text = "Ready";
            statusLabel.ForeColor = Color.Green;
        }

        private void AppendToChatDisplay(string text)
        {
            chatDisplayTextBox.AppendText(text);
            chatDisplayTextBox.SelectionStart = chatDisplayTextBox.Text.Length;
            chatDisplayTextBox.ScrollToCaret();
        }

        public void AddSystemMessage(string message)
        {
            AppendToChatDisplay($"System: {message}\r\n\r\n");
        }

        private void chatDisplayTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}