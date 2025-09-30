using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoBeau.Services;
using AutoBeau.Common;
using AutoBeau.MCP; // Added for MCP client

namespace AutoBeau.Forms
{
    public partial class ChatWindow : UserControl
    {
        private TextBox chatDisplayTextBox;
        private TextBox chatInputTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button clearButton;
        private Label statusLabel;
        private AIChatService _aiChatService;
        private PictureBox statusIcon;
        private List<string> _chatHistory;
        private InventorMCPClient _mcpClient = new InventorMCPClient(); // MCP client instance

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
            this.statusIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).BeginInit();
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
            this.chatDisplayTextBox.Size = new System.Drawing.Size(644, 520);
            this.chatDisplayTextBox.TabIndex = 0;
            this.chatDisplayTextBox.Text = "AI Chat Assistant - Ask me anything about your Inventor project!\r\n\r\n";
            this.chatDisplayTextBox.TextChanged += new System.EventHandler(this.chatDisplayTextBox_TextChanged);
            // 
            // chatInputTextBox
            // 
            this.chatInputTextBox.AcceptsReturn = true;
            this.chatInputTextBox.AcceptsTab = true;
            this.chatInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatInputTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatInputTextBox.Location = new System.Drawing.Point(10, 558);
            this.chatInputTextBox.Multiline = true;
            this.chatInputTextBox.Name = "chatInputTextBox";
            this.chatInputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatInputTextBox.Size = new System.Drawing.Size(644, 86);
            this.chatInputTextBox.TabIndex = 1;
            this.chatInputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChatInputTextBox_KeyDown);
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.BackColor = System.Drawing.Color.Transparent;
            this.sendButton.BackgroundImage = global::AutoBeau.Properties.Resources.send;
            this.sendButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.sendButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sendButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Menu;
            this.sendButton.FlatAppearance.BorderSize = 0;
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.ForeColor = System.Drawing.Color.Black;
            this.sendButton.Location = new System.Drawing.Point(553, 654);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(85, 37);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "\r\n";
            this.sendButton.UseVisualStyleBackColor = false;
            this.sendButton.Visible = false;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.BackColor = System.Drawing.Color.Transparent;
            this.clearButton.BackgroundImage = global::AutoBeau.Properties.Resources.broom;
            this.clearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.clearButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.clearButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.clearButton.FlatAppearance.BorderSize = 0;
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.ForeColor = System.Drawing.Color.Black;
            this.clearButton.Location = new System.Drawing.Point(457, 652);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(106, 40);
            this.clearButton.TabIndex = 3;
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.Gray;
            this.statusLabel.Location = new System.Drawing.Point(45, 662);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(72, 30);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Ready";
            // 
            // statusIcon
            // 
            this.statusIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusIcon.Location = new System.Drawing.Point(10, 662);
            this.statusIcon.Name = "statusIcon";
            this.statusIcon.Size = new System.Drawing.Size(27, 27);
            this.statusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.statusIcon.TabIndex = 5;
            this.statusIcon.TabStop = false;
            this.statusIcon.Click += new System.EventHandler(this.statusIcon_Click);
            // 
            // ChatWindow
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Controls.Add(this.statusIcon);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatInputTextBox);
            this.Controls.Add(this.chatDisplayTextBox);
            this.Name = "ChatWindow";
            this.Size = new System.Drawing.Size(664, 703);
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).EndInit();
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
                    SetStatusIcon("ready");
                    //AppendToChatDisplay("AI service connected using saved API key!\r\n\r\n");
                }
                catch (Exception ex)
                {
                    statusLabel.Text = "AI Unavailable";
                    statusLabel.ForeColor = Color.Red;
                    SetStatusIcon("error");
                    AppendToChatDisplay($"Error initializing AI service with saved key: {ex.Message}\r\n\r\n");
                }
            }
            else
            {
                statusLabel.Text = "API Key Required";
                statusLabel.ForeColor = Color.Orange;
                SetStatusIcon("warning");
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
                SetStatusIcon("ready");
                AppendToChatDisplay("AI service connected successfully!\r\n");

                // Save the API key for future use
                ConfigurationHelper.SaveApiKey(apiKey);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "AI Unavailable";
                statusLabel.ForeColor = Color.Red;
                SetStatusIcon("error");
                AppendToChatDisplay($"Error connecting to AI service: {ex.Message}\r\n");
            }
        }

        private void SetStatusIcon(string status)
        {
            try
            {
                switch (status.ToLower())
                {
                    case "ready":
                    case "success":
                        statusIcon.Image = AutoBeau.Properties.Resources.check;
                        break;
                    case "thinking":
                    case "processing":
                        statusIcon.Image = AutoBeau.Properties.Resources.brain;
                        break;
                    case "error":
                    case "failed":
                        statusIcon.Image = AutoBeau.Properties.Resources.circle;
                        break;
                    case "warning":
                    case "caution":
                        statusIcon.Image = AutoBeau.Properties.Resources.warning_sign;
                        break;
                    default:
                        statusIcon.Image = null;
                        break;
                }

                // Force the PictureBox to refresh
                statusIcon.Invalidate();
                statusIcon.Update();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting status icon ({status}): {ex.Message}");
                // Fallback: clear the icon if there's an error
                statusIcon.Image = null;
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

            // Check for MCP slash command before AI handling
            if (userMessage.StartsWith("/"))
            {
                chatInputTextBox.Clear();
                AppendToChatDisplay($"User: {userMessage}\r\n");
                _chatHistory.Add($"User: {userMessage}");
                if (TryHandleCommand(userMessage))
                {
                    return; // Command handled; skip AI
                }
                // If not recognized, note and fall through to AI
            }

            if (_aiChatService == null)
            {
                AppendToChatDisplay("AI service not available. Please check your API key.\r\n");
                return;
            }

            // Regular AI path
            chatInputTextBox.Clear();
            if (!userMessage.StartsWith("/"))
            {
                AppendToChatDisplay($"User: {userMessage}\r\n");
                _chatHistory.Add($"User: {userMessage}");
            }

            statusLabel.Text = "Thinking...";
            statusLabel.ForeColor = Color.Orange;
            SetStatusIcon("thinking");
            sendButton.Enabled = false;

            try
            {
                string contextSummary = InventorContextService.Instance.GetContextSummary();
                string response = await _aiChatService.SendMessageWithContextAsync(userMessage, _chatHistory.ToArray(), contextSummary);
                AppendToChatDisplay($"AI: {response}\r\n\r\n");
                _chatHistory.Add($"AI: {response}");
                statusLabel.Text = "Ready";
                statusLabel.ForeColor = Color.Green;
                SetStatusIcon("ready");
            }
            catch (Exception ex)
            {
                AppendToChatDisplay($"Error: {ex.Message}\r\n\r\n");
                statusLabel.Text = "Error occurred";
                statusLabel.ForeColor = Color.Red;
                SetStatusIcon("error");
            }
            finally
            {
                sendButton.Enabled = true;
                chatInputTextBox.Focus();
            }
        }

        private bool TryHandleCommand(string cmd)
        {
            try
            {
                string lower = cmd.ToLowerInvariant();
                if (lower == "/help")
                {
                    AppendToChatDisplay("MCP: Commands:\r\n  /methods\r\n  /select rd=1 aa=0 ht=1 cm=0\r\n  /exec <method name> [view]\r\n  /execsel\r\n  /help\r\n\r\n");
                    return true;
                }
                if (lower == "/methods") { AppendToChatDisplay(FormatMethods(_mcpClient.ListMethods())); return true; }
                if (lower.StartsWith("/select"))
                {
                    bool rd=false, aa=false, ht=false, cm=false;
                    foreach (var p in cmd.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (p.StartsWith("rd=", StringComparison.OrdinalIgnoreCase)) rd = EndsWithOne(p);
                        else if (p.StartsWith("aa=", StringComparison.OrdinalIgnoreCase)) aa = EndsWithOne(p);
                        else if (p.StartsWith("ht=", StringComparison.OrdinalIgnoreCase)) ht = EndsWithOne(p);
                        else if (p.StartsWith("cm=", StringComparison.OrdinalIgnoreCase)) cm = EndsWithOne(p);
                    }
                    _mcpClient.SetSelections(rd, aa, ht, cm);
                    AppendToChatDisplay($"MCP: Selections updated rd={rd} aa={aa} ht={ht} cm={cm}\r\n\r\n");
                    return true;
                }
                if (lower.StartsWith("/execsel"))
                {
                    AppendToChatDisplay(FormatResult("Execute Selected", _mcpClient.ExecuteSelectedQueue()));
                    return true;
                }
                if (lower.StartsWith("/exec"))
                {
                    var argText = cmd.Substring(5).Trim();
                    if (string.IsNullOrEmpty(argText)) { AppendToChatDisplay("MCP: Usage: /exec <method name> [view]\r\n\r\n"); return true; }

                    string resolvedMethod; string resolvedView;
                    if (!ResolveMethodAndView(argText, out resolvedMethod, out resolvedView))
                    {
                        AppendToChatDisplay("MCP: Method not recognized. Try /methods or use quotes.\r\n\r\n");
                        return true;
                    }
                    AppendToChatDisplay(FormatResult(resolvedMethod, _mcpClient.ExecuteMethod(resolvedMethod, resolvedView)));
                    return true;
                }
                if (lower.StartsWith("/")) { AppendToChatDisplay("MCP: Unknown command. Type /help\r\n\r\n"); return true; }
            }
            catch (Exception ex)
            {
                AppendToChatDisplay($"MCP Error: {ex.Message}\r\n\r\n");
                return true;
            }
            return false;
        }

        // Resolve method name and optional view from free-form argument text.
        private bool ResolveMethodAndView(string argText, out string methodName, out string viewName)
        {
            methodName = null; viewName = null;
            var methods = _mcpClient.ListMethods();
            var methodNames = new List<string>();
            for (int i = 0; i < methods.Count; i++)
            {
                try
                {
                    var t = methods[i].GetType();
                    var n = Convert.ToString(t.GetProperty("name").GetValue(methods[i], null));
                    if (!string.IsNullOrEmpty(n)) methodNames.Add(n);
                }
                catch { }
            }
            string trimmed = argText.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\"") && trimmed.Length > 1)
            {
                string inner = trimmed.Substring(1, trimmed.Length - 2).Trim();
                methodName = methodNames.FirstOrDefault(m => m.Equals(inner, StringComparison.OrdinalIgnoreCase));
                return methodName != null;
            }
            methodName = methodNames.FirstOrDefault(m => m.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
            if (methodName != null) return true;
            var candidates = methodNames.Where(m => m.StartsWith(trimmed, StringComparison.OrdinalIgnoreCase)).ToList();
            if (candidates.Count == 1) { methodName = candidates[0]; return true; }
            if (candidates.Count > 1) { methodName = candidates.OrderBy(c => c.Length).First(); return true; }
            candidates = methodNames.Where(m => m.IndexOf(trimmed, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (candidates.Count == 1) { methodName = candidates[0]; return true; }
            var tokens = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 1)
            {
                for (int len = tokens.Length; len >= 1; len--)
                {
                    string join = string.Join(" ", tokens.Take(len));
                    var exact = methodNames.FirstOrDefault(m => m.Equals(join, StringComparison.OrdinalIgnoreCase));
                    if (exact != null)
                    {
                        methodName = exact;
                        if (len < tokens.Length) viewName = string.Join(" ", tokens.Skip(len));
                        return true;
                    }
                }
            }
            if (tokens.Length == 1)
            {
                string word = tokens[0];
                candidates = methodNames.Where(m => m.Split(' ').Any(w => w.StartsWith(word, StringComparison.OrdinalIgnoreCase))).ToList();
                if (candidates.Count == 1) { methodName = candidates[0]; return true; }
            }
            return false;
        }

        // Helper methods re-added
        private bool EndsWithOne(string token) => token.EndsWith("=1") || token.EndsWith("=true", StringComparison.OrdinalIgnoreCase);

        private string FormatMethods(IList<object> list)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("MCP: Methods:");
            foreach (var item in list)
            {
                try
                {
                    var t = item.GetType();
                    sb.AppendLine($"  - {t.GetProperty("name")?.GetValue(item, null)} (priority {t.GetProperty("priority")?.GetValue(item, null)}) selected={t.GetProperty("selected")?.GetValue(item, null)}");
                }
                catch { sb.AppendLine("  - <unreadable item>"); }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        private string FormatResult(string title, object result)
        {
            try
            {
                var t = result.GetType();
                var okProp = t.GetProperty("ok");
                if (okProp == null) return "MCP: (unrecognized result)\r\n\r\n";
                bool ok = Convert.ToBoolean(okProp.GetValue(result, null));
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"MCP: {title} -> {(ok ? "success" : "failure")}");
                string method = t.GetProperty("method")?.GetValue(result, null) as string;
                string message = t.GetProperty("message")?.GetValue(result, null) as string;
                string error = t.GetProperty("error")?.GetValue(result, null) as string;
                if (!string.IsNullOrEmpty(method)) sb.AppendLine("  method: " + method);
                if (!string.IsNullOrEmpty(message)) sb.AppendLine("  message: " + message);
                if (!string.IsNullOrEmpty(error)) sb.AppendLine("  error: " + error);
                var resultsProp = t.GetProperty("results");
                if (resultsProp != null)
                {
                    var coll = resultsProp.GetValue(result, null) as System.Collections.IEnumerable;
                    if (coll != null)
                    {
                        sb.AppendLine("  results:");
                        foreach (var r in coll)
                        {
                            try
                            {
                                var rt = r.GetType();
                                sb.AppendLine($"    - {rt.GetProperty("method")?.GetValue(r, null)}: success={rt.GetProperty("success")?.GetValue(r, null)} warning={rt.GetProperty("warning")?.GetValue(r, null)} error={rt.GetProperty("error")?.GetValue(r, null)} msg={rt.GetProperty("message")?.GetValue(r, null)}");
                            }
                            catch { sb.AppendLine("    - <unreadable>"); }
                        }
                    }
                }
                sb.AppendLine();
                return sb.ToString();
            }
            catch (Exception ex) { return "MCP: (format error) " + ex.Message + "\r\n\r\n"; }
        }

        private List<string> ParseArguments(string text)
        {
            var args = new List<string>();
            if (string.IsNullOrWhiteSpace(text)) return args;
            bool inQuote = false; var current = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (c == '"') { inQuote = !inQuote; continue; }
                if (!inQuote && char.IsWhiteSpace(c))
                {
                    if (current.Length > 0) { args.Add(current.ToString()); current.Clear(); }
                }
                else current.Append(c);
            }
            if (current.Length > 0) args.Add(current.ToString());
            return args;
        }

        // RESTORED / ORIGINAL SUPPORT METHODS ------------------------------
        private void ClearButton_Click(object sender, EventArgs e)
        {
            chatDisplayTextBox.Clear();
            _chatHistory.Clear();
            chatDisplayTextBox.Text = "AI Chat Assistant - Ask me anything about your Inventor project!\r\n\r\n";
            statusLabel.Text = "Ready";
            statusLabel.ForeColor = Color.Green;
            SetStatusIcon("ready");
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
            // Intentionally left blank ¨C event maintained for designer compatibility
        }

        private void statusIcon_Click(object sender, EventArgs e)
        {
            // Placeholder for future status details popup
        }
    }
}
