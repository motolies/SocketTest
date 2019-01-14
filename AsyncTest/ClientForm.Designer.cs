namespace AsyncTest
{
    partial class ClientForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbDebug = new System.Windows.Forms.TextBox();
            this.btHello = new System.Windows.Forms.Button();
            this.btHI = new System.Windows.Forms.Button();
            this.btnAn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbDebug
            // 
            this.tbDebug.Location = new System.Drawing.Point(12, 66);
            this.tbDebug.Multiline = true;
            this.tbDebug.Name = "tbDebug";
            this.tbDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDebug.Size = new System.Drawing.Size(420, 184);
            this.tbDebug.TabIndex = 0;
            // 
            // btHello
            // 
            this.btHello.Location = new System.Drawing.Point(12, 12);
            this.btHello.Name = "btHello";
            this.btHello.Size = new System.Drawing.Size(99, 48);
            this.btHello.TabIndex = 1;
            this.btHello.Text = "HELLO";
            this.btHello.UseVisualStyleBackColor = true;
            // 
            // btHI
            // 
            this.btHI.Location = new System.Drawing.Point(117, 12);
            this.btHI.Name = "btHI";
            this.btHI.Size = new System.Drawing.Size(99, 48);
            this.btHI.TabIndex = 1;
            this.btHI.Text = "HI";
            this.btHI.UseVisualStyleBackColor = true;
            // 
            // btnAn
            // 
            this.btnAn.Location = new System.Drawing.Point(222, 12);
            this.btnAn.Name = "btnAn";
            this.btnAn.Size = new System.Drawing.Size(99, 48);
            this.btnAn.TabIndex = 2;
            this.btnAn.Text = "안녕";
            this.btnAn.UseVisualStyleBackColor = true;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 262);
            this.Controls.Add(this.btnAn);
            this.Controls.Add(this.btHI);
            this.Controls.Add(this.btHello);
            this.Controls.Add(this.tbDebug);
            this.Name = "ClientForm";
            this.Text = "Async Socket";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDebug;
        private System.Windows.Forms.Button btHello;
        private System.Windows.Forms.Button btHI;
        private System.Windows.Forms.Button btnAn;
    }
}

