# -*- coding: utf-8 -*-
"""
Full report generator for Personal Expense Tracker (Quản lý chi tiêu cá nhân)
Author: Nguyễn Văn Mạnh (MSV: 2321050012)
Instructor: ThS. Ngô Hùng Long
School: Đại học Mỏ - Địa chất (HUMG) - Khoa CNTT
"""

import sys
sys.stdout.reconfigure(encoding='utf-8')
import os
import shutil
import docx
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.oxml import parse_xml
from docx.oxml.ns import nsdecls
from report_helpers import (
    add_h1, add_h2, add_h3, add_p, add_bullet, add_subbullet,
    add_code_placeholder, add_ui_placeholder, add_table_data,
    COLOR_NAVY, COLOR_TEXT
)

def generate_btl_report():
    template_path = "Báo cáo BTL Net.1_2221050405_Nguyễn Thị An 1.docx"
    output_path = "Báo cáo BTL Net.1_2321050012_Nguyễn Văn Mạnh.docx"

    print("Copying template to output...")
    shutil.copy(template_path, output_path)
    doc = docx.Document(output_path)

    # 1. Update Cover Page (Paragraphs 0 to 17)
    p8 = doc.paragraphs[8]
    p8.text = "XÂY DỰNG HỆ THỐNG QUẢN LÝ CHI TIÊU CÁ NHÂN"
    p8.alignment = WD_ALIGN_PARAGRAPH.CENTER
    for r in p8.runs:
        r.font.name = "Times New Roman"
        r.font.size = Pt(14)
        r.bold = True

    p10 = doc.paragraphs[10]
    p10.text = "Mã nhóm : 01"
    p10.alignment = WD_ALIGN_PARAGRAPH.CENTER
    for r in p10.runs:
        r.font.name = "Times New Roman"
        r.font.size = Pt(14)
        r.bold = True

    p11 = doc.paragraphs[11]
    p11.text = "Giảng viên hướng dẫn : ThS. Ngô Hùng Long"
    p11.alignment = WD_ALIGN_PARAGRAPH.CENTER
    for r in p11.runs:
        r.font.name = "Times New Roman"
        r.font.size = Pt(14)
        r.bold = True

    p12 = doc.paragraphs[12]
    p12.text = "Sinh viên thực hiện : Nguyễn Văn Mạnh"
    p12.alignment = WD_ALIGN_PARAGRAPH.CENTER
    for r in p12.runs:
        r.font.name = "Times New Roman"
        r.font.size = Pt(14)
        r.bold = True

    p13 = doc.paragraphs[13]
    p13.text = "Mã sinh viên : 2321050012"
    p13.alignment = WD_ALIGN_PARAGRAPH.CENTER
    for r in p13.runs:
        r.font.name = "Times New Roman"
        r.font.size = Pt(14)
        r.bold = True

    p14 = doc.paragraphs[14]
    p14.text = "Lớp : DCCTCT68_05B"
    p14.alignment = WD_ALIGN_PARAGRAPH.CENTER
    for r in p14.runs:
        r.font.name = "Times New Roman"
        r.font.size = Pt(14)
        r.bold = True

    p16 = doc.paragraphs[16]
    p16.text = "Hà Nội – 2026"
    p16.alignment = WD_ALIGN_PARAGRAPH.CENTER
    for r in p16.runs:
        r.font.name = "Times New Roman"
        r.font.size = Pt(14)

    # 2. Clear old body content
    body = doc._body._element
    elements_to_remove = [body[i] for i in range(18, len(body)-1)]
    for el in elements_to_remove:
        body.remove(el)

    # Configure Section 1 margins (Academic thesis standard: Top 2cm, Bottom 2cm, Left 3cm, Right 2cm)
    sec1 = doc.sections[1]
    sec1.top_margin = Inches(0.7875)
    sec1.bottom_margin = Inches(0.7875)
    sec1.left_margin = Inches(1.18125)
    sec1.right_margin = Inches(0.7875)

    # Add page number to Section 1 footer
    sec1.footer.is_linked_to_previous = False
    footer_p = sec1.footer.paragraphs[0]
    footer_p.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    run_f = footer_p.add_run()
    run_f.font.name = "Times New Roman"
    run_f.font.size = Pt(11)
    run_f.font.color.rgb = RGBColor(100, 100, 100)

    fld1 = parse_xml(f'<w:fldChar {nsdecls("w")} w:fldCharType="begin"/>')
    instr = parse_xml(f'<w:instrText {nsdecls("w")} xml:space="preserve"> PAGE </w:instrText>')
    fld2 = parse_xml(f'<w:fldChar {nsdecls("w")} w:fldCharType="separate"/>')
    fld3 = parse_xml(f'<w:fldChar {nsdecls("w")} w:fldCharType="end"/>')
    run_f._r.append(fld1)
    run_f._r.append(instr)
    run_f._r.append(fld2)
    run_f._r.append(fld3)

    print("Building report sections...")

    # =========================================================================
    # CHƯƠNG 1: GIỚI THIỆU ĐỀ TÀI
    # =========================================================================
    add_h1(doc, "1. Giới thiệu đề tài")
    
    add_h2(doc, "1.1. Lý do chọn đề tài")
    add_p(doc, 
        "Trong bối cảnh kinh tế hiện đại và sự phát triển mạnh mẽ của xã hội số, quản lý tài chính cá nhân đã trở thành "
        "một kỹ năng sống còn đối với mỗi cá nhân, từ học sinh, sinh viên cho đến những người đã đi làm. Việc quản lý thu chi "
        "hợp lý không chỉ giúp đảm bảo cuộc sống ổn định, tránh rơi vào khủng hoảng nợ nần mà còn tạo nền tảng vững chắc để thực "
        "hiện các mục tiêu tương lai như đầu tư, tích lũy và tự do tài chính. Tuy nhiên, trong thực tế hiện nay, phần lớn mọi người "
        "vẫn duy trì thói quen theo dõi chi tiêu một cách tự phát, không có kế hoạch rõ ràng hoặc chỉ ghi chép sơ sài trên sổ tay, "
        "ứng dụng ghi chú (Notepad) hay các bảng tính Excel rời rạc.")
    
    add_p(doc, 
        "Phương pháp ghi chép thủ công bộc lộ vô số nhược điểm cố hữu: tốn thời gian, dễ nhầm lẫn số liệu, không thể tự động tổng hợp "
        "báo cáo định kỳ và đặc biệt là không thể đưa ra những cảnh báo tức thời khi chi tiêu vượt quá mức kiểm soát. Người dùng "
        "rất dễ rơi vào tình trạng 'cháy túi' vào cuối tháng mà không hề biết tiền của mình đã biến mất vào những khoản mục nào. Mặt khác, "
        "các khoản vay mượn nhỏ lẻ từ bạn bè, người thân thường xuyên bị lãng quên hoặc nhầm lẫn do thiếu công cụ theo dõi trạng thái trả nợ "
        "riêng biệt. Đối với các phần mềm trên thị trường, nhiều ứng dụng chứa quá nhiều quảng cáo gây khó chịu, yêu cầu liên kết trực tiếp "
        "tài khoản ngân hàng dẫn đến nguy cơ mất an toàn thông tin hoặc áp dụng mức phí thuê bao hàng tháng đắt đỏ.")

    add_p(doc, 
        "Trước thực trạng đó, việc xây dựng một hệ thống phần mềm quản lý chi tiêu cá nhân (Personal Expense Tracker) chuyên biệt "
        "trên nền tảng Windows Forms (.NET 10) kết hợp hệ quản trị cơ sở dữ liệu PostgreSQL theo kiến trúc 3 lớp (3-Tier Architecture) "
        "là một đề tài mang tính thời sự, thực tiễn và ứng dụng cao. Phần mềm không chỉ giải quyết trọn vẹn bài toán ghi nhận thu chi, "
        "quản lý danh mục, thiết lập hạn mức ngân sách thông minh kèm cảnh báo tự động, mà còn tích hợp các tiện ích vượt trội như quản lý "
        "sổ vay nợ đối ứng, khôi phục mật khẩu bảo mật qua mã OTP gửi qua Gmail SMTP và trực quan hóa toàn bộ bức tranh tài chính qua hệ "
        "thống biểu đồ phân tích đa chiều.")

    add_h2(doc, "1.2. Mục tiêu của đề tài")
    add_p(doc, 
        "Mục tiêu cốt lõi của đề tài là nghiên cứu và xây dựng hoàn chỉnh một ứng dụng máy tính (Desktop Application) chuyên nghiệp "
        "với các tiêu chí cụ thể sau:")
    add_bullet(doc, "Cung cấp hệ thống đăng ký, đăng nhập an toàn, bảo mật mật khẩu bằng thuật toán mã hóa hiện đại và tính năng quên mật khẩu xác thực mã OTP 6 số qua giao thức SMTP Gmail.", "Bảo mật & Quản lý người dùng: ")
    add_bullet(doc, "Cho phép phân loại các khoản tiền thành Thu nhập và Chi tiêu theo từng danh mục cụ thể (Ăn uống, Đi lại, Mua sắm, Lương, Thưởng,...), hỗ trợ thêm, sửa, xóa linh hoạt.", "Quản lý danh mục linh hoạt: ")
    add_bullet(doc, "Ghi nhận nhanh chóng các giao dịch phát sinh hàng ngày kèm thời gian, danh mục và ghi chú; tự động định dạng tiền tệ và kiểm tra tính hợp lệ của dữ liệu.", "Quản lý giao dịch thu chi: ")
    add_bullet(doc, "Người dùng có thể đặt hạn mức chi tiêu tối đa cho từng danh mục theo tháng/năm; hệ thống tự động kiểm tra và đưa ra cảnh báo mức vàng (80%) hoặc cảnh báo đỏ (100%) trước khi lưu giao dịch.", "Thiết lập ngân sách & Cảnh báo vượt mức: ")
    add_bullet(doc, "Theo dõi danh sách các khoản 'Tôi Nợ' và 'Cho Vay', hỗ trợ thanh toán nợ từng phần hoặc toàn phần và tự động cập nhật đối ứng vào lịch sử giao dịch thu chi của ví.", "Quản lý khoản vay & Sổ nợ: ")
    add_bullet(doc, "Cung cấp 3 chế độ biểu đồ tài chính trực quan: So sánh thu chi các ngày trong tháng, Báo cáo thu chi 12 tháng trong năm và Biểu đồ tròn phân tích tỷ trọng cơ cấu chi tiêu.", "Thống kê & Trực quan hóa dữ liệu: ")
    add_bullet(doc, "Thiết kế màn hình Dashboard tổng quan hỗ trợ chuyển đổi tháng/năm linh hoạt, tính toán số dư tức thì và phân tích cột 'Nhìn Lại' phân loại thu chi theo màu sắc sinh động.", "Trải nghiệm người dùng (UX/UI): ")

    add_h2(doc, "1.3. Phạm vi thực hiện")
    add_p(doc, "Phạm vi chức năng phần mềm tập trung giải quyết:", bold_prefix="• Phạm vi thực hiện: ", indent=False)
    add_bullet(doc, "Áp dụng cho đối tượng người dùng cá nhân (mỗi tài khoản quản lý độc lập toàn bộ dữ liệu tài chính của riêng mình).")
    add_bullet(doc, "Quản lý chi tiết toàn bộ chu kỳ sống của giao dịch thu chi: nhập mới, tra cứu lịch sử, tìm kiếm theo từ khóa, lọc theo loại, chỉnh sửa và xóa bỏ.")
    add_bullet(doc, "Thiết lập ngân sách tháng và theo dõi tiến độ chi tiêu qua thanh chỉ số trực quan (ProgressBar) có phân cấp màu sắc cảnh báo.")
    add_bullet(doc, "Theo dõi các khoản vay mượn cá nhân, tự động ghi nhận giao dịch khi hoàn tất thanh toán nợ.")
    add_bullet(doc, "Tổng hợp số liệu và vẽ biểu đồ phân tích tài chính đa chiều.")
    
    add_p(doc, "Những nội dung không thuộc phạm vi của đề tài:", bold_prefix="• Phạm vi không thực hiện: ", indent=False)
    add_bullet(doc, "Không liên kết trực tiếp với cổng thanh toán API của các ngân hàng thương mại (người dùng tự chủ động ghi nhận các khoản thu chi phát sinh).")
    add_bullet(doc, "Không thực hiện các nghiệp vụ kế toán doanh nghiệp phức tạp như tính thuế thu nhập doanh nghiệp, tính thuế GTGT hay trích khấu hao tài sản.")
    add_bullet(doc, "Không tích hợp quản lý ví đầu tư tiền mã hóa (Cryptocurrency) hay theo dõi biến động thị trường chứng khoán theo thời gian thực.")
    add_bullet(doc, "Hệ thống hoạt động độc lập trên môi trường Windows Desktop (WinForms) kết nối CSDL PostgreSQL cục bộ/mạng nội bộ.")

    # =========================================================================
    # CHƯƠNG 2: PHÂN TÍCH YÊU CẦU
    # =========================================================================
    add_h1(doc, "2. Phân tích yêu cầu")

    add_h2(doc, "2.1. Mô tả bài toán")
    add_p(doc, 
        "Hệ thống hướng tới việc phục vụ cá nhân cần kiểm soát chi tiêu sinh hoạt, xây dựng quỹ tiết kiệm và duy trì kỷ luật tài chính. "
        "Mỗi người dùng khi tiếp cận phần mềm sẽ sở hữu một không gian dữ liệu riêng biệt được bảo vệ bằng thông tin đăng nhập cá nhân (Email và Mật khẩu). "
        "Bài toán nghiệp vụ cốt lõi xoay quanh các tương tác sau:")
    add_bullet(doc, "Khởi tạo tài khoản cá nhân, cập nhật hồ sơ (Họ tên, Ngày sinh, Giới tính, Địa chỉ) và sử dụng cơ chế khôi phục mật khẩu qua Email OTP khi quên mật khẩu.", "Người dùng: ")
    add_bullet(doc, "Tạo lập các danh mục thu nhập (Lương, Thưởng, Tiền phụ cấp, Đầu tư,...) và danh mục chi tiêu (Ăn uống, Nhà ở, Mua sắm, Di chuyển, Y tế, Giáo dục, Giải trí,...).", "Cấu hình danh mục: ")
    add_bullet(doc, "Thực hiện nhập các khoản thu chi phát sinh trong ngày. Hệ thống tự động kiểm tra số tiền này so với hạn mức ngân sách tháng đã đặt ra để cảnh báo người dùng trước khi lưu trữ.", "Ghi nhận dòng tiền: ")
    add_bullet(doc, "Theo dõi các khoản tiền mình vay của người khác hoặc cho bạn bè vay. Khi nhận lại tiền hoặc trả nợ, người dùng thực hiện thao tác 'Thanh toán', hệ thống tự động đổi trạng thái và sinh giao dịch thu/chi tương ứng.", "Theo dõi công nợ: ")
    add_bullet(doc, "Xem màn hình Dashboard và báo cáo Thống kê để nhận biết tổng thu, tổng chi, số tiền tiết kiệm còn lại, cũng như theo dõi tỷ trọng các khoản chi lớn nhất trong tháng để có kế hoạch cắt giảm hợp lý.", "Phân tích tài chính: ")

    add_h2(doc, "2.2. Yêu cầu chức năng")
    add_p(doc, "Hệ thống được thiết kế hoàn chỉnh với 7 phân hệ chức năng chính sau:")

    add_bullet(doc, "Cho phép người dùng tạo tài khoản mới bằng Email và mật khẩu; Đăng nhập vào hệ thống; Đăng xuất an toàn.", "1. Phân hệ Xác thực & Tài khoản: ")
    add_subbullet(doc, "Tính năng 'Quên Mật Khẩu': gửi mã OTP ngẫu nhiên gồm 6 chữ số qua Gmail SMTP đến hộp thư người dùng với thời hạn hiệu lực 5 phút.")
    add_subbullet(doc, "Xác minh mã OTP và chuyển hướng tới giao diện đặt lại mật khẩu mới an toàn.")
    add_subbullet(doc, "Quản lý hồ sơ cá nhân: xem và chỉnh sửa họ tên, giới tính, ngày sinh, địa chỉ; đổi mật khẩu hiện tại.")

    add_bullet(doc, "Quản lý danh sách các nhóm thu và chi riêng biệt của từng người dùng; hỗ trợ Thêm mới danh mục, Chỉnh sửa tên/loại danh mục và Xóa danh mục không còn sử dụng.", "2. Phân hệ Quản lý Danh mục: ")

    add_bullet(doc, "Ghi nhận giao dịch phát sinh với số tiền, loại (Thu nhập / Chi tiêu), danh mục tương ứng, thời gian và ghi chú chi tiết.", "3. Phân hệ Quản lý Giao dịch: ")
    add_subbullet(doc, "Hỗ trợ tự động định dạng phân tách hàng nghìn khi nhập số tiền (ví dụ: 100,000 đ).")
    add_subbullet(doc, "Kiểm tra và kích hoạt cảnh báo ngân sách tự động trước khi lưu giao dịch.")
    add_subbullet(doc, "Tra cứu lịch sử giao dịch (Màn hình Lịch): tìm kiếm theo nội dung ghi chú, lọc theo loại giao dịch (Tất cả / Thu Nhập / Chi Tiêu).")
    add_subbullet(doc, "Chỉnh sửa thông tin giao dịch đã ghi hoặc Xóa giao dịch kèm hộp thoại xác nhận.")

    add_bullet(doc, "Cho phép người dùng đặt hạn mức chi tiêu tối đa cho từng danh mục chi tiêu theo tháng và năm cụ thể.", "4. Phân hệ Thiết lập Ngân sách: ")
    add_subbullet(doc, "Tính toán tỷ lệ phần trăm đã chi tiêu so với hạn mức và số tiền còn lại có thể chi.")
    add_subbullet(doc, "Hiển thị trực quan qua thanh tiến độ ProgressBar có màu sắc nhận biết (Dưới 80%: Xanh an toàn; Từ 80% - 100%: Vàng cảnh báo; Vượt 100%: Đỏ nguy hiểm).")

    add_bullet(doc, "Theo dõi danh sách các khoản 'Tôi Nợ' (nợ phải trả) và 'Cho Vay' (nợ phải thu) kèm tên đối tác, số tiền, ngày hẹn trả và ghi chú.", "5. Phân hệ Quản lý Vay & Nợ: ")
    add_subbullet(doc, "Bộ lọc theo loại nợ và trạng thái (Đang nợ / Đã trả).")
    add_subbullet(doc, "Chức năng 'Thanh toán nợ' (Settle Debt): chuyển trạng thái sang Đã Trả, đồng thời tự động chèn một giao dịch Thu Nhập hoặc Chi Tiêu vào lịch sử để dòng tiền luôn được đồng bộ.")

    add_bullet(doc, "Cung cấp điều hướng chọn Tháng/Năm linh hoạt; Tự động tính toán Thẻ Tổng Thu, Thẻ Tổng Chi và Thẻ Còn Lại.", "6. Phân hệ Dashboard Tổng quan: ")
    add_subbullet(doc, "Hiển thị danh sách các giao dịch gần đây phát sinh trong tháng.")
    add_subbullet(doc, "Cột 'Nhìn Lại': phân tích chi tiết tổng thu và tổng chi theo từng danh mục trong tháng, phân biệt màu sắc xanh lá (Thu Nhập) và màu đỏ (Chi Tiêu).")

    add_bullet(doc, "Vẽ biểu đồ phân tích tài chính đa dạng bằng thư viện System.Windows.Forms.DataVisualization.Charting.", "7. Phân hệ Báo cáo & Thống kê: ")
    add_subbullet(doc, "Chế độ 1: Biểu đồ cột so sánh hai luồng Thu Nhập và Chi Tiêu của từng ngày trong tháng được chọn.")
    add_subbullet(doc, "Chế độ 2: Biểu đồ cột tổng hợp thu chi của cả 12 tháng trong năm.")
    add_subbullet(doc, "Chế độ 3: Biểu đồ hình tròn (Pie Chart) thể hiện tỷ trọng cơ cấu phần trăm chi tiêu của từng danh mục trong tháng.")

    add_h2(doc, "2.3. Yêu cầu phi chức năng")
    add_bullet(doc, "Hệ thống phản hồi tức thì các thao tác người dùng (thời gian xử lý truy vấn cơ sở dữ liệu dưới 0.5 giây đối với tập dữ liệu hàng nghìn giao dịch).", "Hiệu năng: ")
    add_bullet(doc, "Mật khẩu người dùng được băm an toàn; kết nối gửi OTP bảo mật tuyệt đối qua giao thức SSL/TLS của máy chủ Google SMTP.", "Bảo mật: ")
    add_bullet(doc, "Cơ sở dữ liệu PostgreSQL đảm bảo tính toàn vẹn tham chiếu (Foreign Keys), cơ chế khóa giao dịch ACID và lưu vết thời gian chính xác.", "Toàn vẹn dữ liệu: ")
    add_bullet(doc, "Giao diện hiện đại, bố cục khoa học, phối màu trang nhã (Tone xanh Indigo/Navy sang trọng), dễ thao tác kể cả với người không am hiểu công nghệ.", "Tính khả dụng: ")

    # =========================================================================
    # CHƯƠNG 3: THIẾT KẾ HỆ THỐNG
    # =========================================================================
    add_h1(doc, "3. Thiết kế hệ thống")

    add_h2(doc, "3.1. Thiết kế cơ sở dữ liệu")
    add_p(doc, 
        "Hệ thống sử dụng hệ quản trị cơ sở dữ liệu quan hệ mã nguồn mở mạnh mẽ nhất hiện nay là PostgreSQL (phiên bản 16). "
        "PostgreSQL hỗ trợ đầy đủ các chuẩn SQL hiện đại, kiểu dữ liệu đa dạng (SERIAL, TIMESTAMP, DECIMAL, JSON) và tính toàn vẹn "
        "dữ liệu nghiêm ngặt. Cơ sở dữ liệu của phần mềm gồm có 5 bảng chính, được thiết kế theo chuẩn hóa dạng 3NF (Third Normal Form) "
        "để loại bỏ triệt để hiện tượng dư thừa dữ liệu.")

    add_p(doc, "Bảng 1: Tổng hợp danh sách các bảng trong hệ thống CSDL PostgreSQL", bold_prefix="", indent=False)
    db_summary_headers = ["Tên bảng", "Khóa chính", "Khóa ngoại", "Mô tả nghiệp vụ"]
    db_summary_data = [
        ["Users", "Id (SERIAL)", "Không có", "Lưu thông tin tài khoản người dùng, email và mật khẩu"],
        ["Categories", "Id (SERIAL)", "UserId -> Users(Id)", "Lưu các danh mục phân loại Thu Nhập và Chi Tiêu"],
        ["Transactions", "Id (SERIAL)", "UserId, CategoryId", "Lưu trữ toàn bộ giao dịch phát sinh dòng tiền thực tế"],
        ["Budgets", "BudgetId (SERIAL)", "UserId, CategoryId", "Lưu thiết lập hạn mức ngân sách chi tiêu theo tháng/năm"],
        ["Debts", "DebtId (SERIAL)", "UserId -> Users(Id)", "Lưu trữ và quản lý các khoản vay nợ và cho vay cá nhân"]
    ]
    add_table_data(doc, db_summary_headers, db_summary_data, [1.3, 1.2, 1.5, 2.3])

    add_h3(doc, "3.1.1. Chi tiết cấu trúc Bảng Users (Tài khoản người dùng)")
    add_p(doc, "Bảng Users đóng vai trò trung tâm trong việc định danh và xác thực quyền sở hữu dữ liệu của từng cá nhân trong hệ thống.")
    users_headers = ["Tên cột", "Kiểu dữ liệu", "Ràng buộc", "Mô tả ý nghĩa"]
    users_data = [
        ["Id", "SERIAL", "PRIMARY KEY", "Mã định danh duy nhất của người dùng"],
        ["FullName", "VARCHAR(150)", "NOT NULL", "Họ và tên hiển thị của người dùng"],
        ["Gender", "VARCHAR(20)", "NULL", "Giới tính (Nam, Nữ, Khác)"],
        ["Dob", "DATE", "NULL", "Ngày tháng năm sinh"],
        ["Email", "VARCHAR(150)", "NOT NULL, UNIQUE", "Email đăng nhập và nhận mã OTP khôi phục"],
        ["Address", "VARCHAR(255)", "NULL", "Địa chỉ liên hệ"],
        ["Password", "VARCHAR(255)", "NOT NULL", "Mật khẩu tài khoản (đã băm bảo mật)"]
    ]
    add_table_data(doc, users_headers, users_data, [1.2, 1.3, 1.6, 2.2])

    add_h3(doc, "3.1.2. Chi tiết cấu trúc Bảng Categories (Danh mục Thu/Chi)")
    add_p(doc, "Bảng Categories lưu các nhóm mục chi tiêu hoặc nguồn thu nhập của người dùng.")
    cats_headers = ["Tên cột", "Kiểu dữ liệu", "Ràng buộc", "Mô tả ý nghĩa"]
    cats_data = [
        ["Id", "SERIAL", "PRIMARY KEY", "Mã định danh danh mục"],
        ["Name", "VARCHAR(100)", "NOT NULL", "Tên danh mục (ví dụ: Ăn uống, Tiền lương,...)"],
        ["Type", "VARCHAR(50)", "NOT NULL", "Loại danh mục: 'Thu Nhập' hoặc 'Chi Tiêu'"],
        ["UserId", "INT", "FOREIGN KEY -> Users(Id)", "Mã người dùng sở hữu danh mục"]
    ]
    add_table_data(doc, cats_headers, cats_data, [1.2, 1.3, 1.8, 2.0])

    add_h3(doc, "3.1.3. Chi tiết cấu trúc Bảng Transactions (Giao dịch thu chi)")
    add_p(doc, "Bảng Transactions ghi nhận toàn bộ biến động tài chính phát sinh hàng ngày của người dùng.")
    trans_headers = ["Tên cột", "Kiểu dữ liệu", "Ràng buộc", "Mô tả ý nghĩa"]
    trans_data = [
        ["Id", "SERIAL", "PRIMARY KEY", "Mã định danh giao dịch duy nhất"],
        ["UserId", "INT", "FOREIGN KEY -> Users(Id)", "Mã người dùng thực hiện giao dịch"],
        ["CategoryId", "INT", "FOREIGN KEY -> Categories(Id)", "Mã danh mục tương ứng với giao dịch"],
        ["Amount", "DECIMAL(18,2)", "NOT NULL", "Số tiền phát sinh trong giao dịch"],
        ["Date", "TIMESTAMP", "NOT NULL", "Thời gian phát sinh giao dịch"],
        ["Description", "VARCHAR(255)", "NULL", "Nội dung ghi chú chi tiết về giao dịch"],
        ["Type", "VARCHAR(50)", "NOT NULL", "Phân loại giao dịch ('Thu Nhập' / 'Chi Tiêu')"]
    ]
    add_table_data(doc, trans_headers, trans_data, [1.2, 1.3, 1.8, 2.0])

    add_h3(doc, "3.1.4. Chi tiết cấu trúc Bảng Budgets (Ngân sách chi tiêu)")
    add_p(doc, "Bảng Budgets lưu trữ các hạn mức chi tiêu tối đa mà người dùng thiết lập cho từng danh mục theo tháng.")
    budgets_headers = ["Tên cột", "Kiểu dữ liệu", "Ràng buộc", "Mô tả ý nghĩa"]
    budgets_data = [
        ["BudgetId", "SERIAL", "PRIMARY KEY", "Mã định danh hạn mức ngân sách"],
        ["UserId", "INT", "FOREIGN KEY -> Users(Id)", "Mã người dùng thiết lập"],
        ["CategoryId", "INT", "FOREIGN KEY -> Categories(Id)", "Mã danh mục áp dụng hạn mức"],
        ["AmountLimit", "DECIMAL(18,2)", "NOT NULL", "Hạn mức số tiền chi tiêu tối đa cho phép"],
        ["Month", "INT", "NOT NULL (1 - 12)", "Tháng áp dụng hạn mức"],
        ["Year", "INT", "NOT NULL", "Năm áp dụng hạn mức"]
    ]
    add_table_data(doc, budgets_headers, budgets_data, [1.2, 1.3, 1.8, 2.0])

    add_h3(doc, "3.1.5. Chi tiết cấu trúc Bảng Debts (Khoản vay & Sổ nợ)")
    add_p(doc, "Bảng Debts lưu trữ các giao dịch công nợ cá nhân để theo dõi nghĩa vụ trả nợ và quyền đòi nợ.")
    debts_headers = ["Tên cột", "Kiểu dữ liệu", "Ràng buộc", "Mô tả ý nghĩa"]
    debts_data = [
        ["DebtId", "SERIAL", "PRIMARY KEY", "Mã định danh khoản nợ"],
        ["UserId", "INT", "FOREIGN KEY -> Users(Id)", "Mã người dùng sở hữu"],
        ["PersonName", "VARCHAR(150)", "NOT NULL", "Tên đối tác (người nợ hoặc chủ nợ)"],
        ["Amount", "DECIMAL(18,2)", "NOT NULL", "Số tiền vay mượn"],
        ["DebtType", "VARCHAR(50)", "NOT NULL", "Phân loại ('Tôi Nợ' / 'Cho Vay')"],
        ["DueDate", "DATE", "NULL", "Hạn thanh toán dự kiến"],
        ["Note", "VARCHAR(255)", "NULL", "Ghi chú mục đích vay mượn"],
        ["Status", "VARCHAR(50)", "NOT NULL", "Trạng thái ('Đang Nợ' / 'Đã Trả')"]
    ]
    add_table_data(doc, debts_headers, debts_data, [1.2, 1.3, 1.8, 2.0])

    # 3.2 Giao diện UI/UX
    add_h2(doc, "3.2. Thiết kế giao diện (UI/UX)")
    add_p(doc, 
        "Giao diện phần mềm được xây dựng theo phong cách Flat Design hiện đại, tối ưu hóa trải nghiệm người dùng (UX) "
        "thông qua hệ thống màu sắc trang nhã, font chữ Segoe UI sắc nét và bố cục đồng nhất giữa các màn hình. "
        "Dưới đây là thiết kế chi tiết của các màn hình chức năng trong ứng dụng:")

    add_h3(doc, "3.2.1. Form Đăng nhập & Đăng ký tài khoản (AuthForm)")
    add_p(doc, 
        "Form gồm hai bảng chuyển đổi linh hoạt: Bảng Đăng nhập (nhập Email, Mật khẩu, nút Đăng nhập, liên kết Quên mật khẩu) "
        "và Bảng Đăng ký (nhập Họ tên, Email, Mật khẩu, Xác nhận mật khẩu, nút Tạo tài khoản).")
    add_ui_placeholder(doc, "Giao diện Form Đăng nhập và Đăng ký tài khoản của hệ thống", "1", "Form Đăng nhập & Đăng ký tài khoản hệ thống")

    add_h3(doc, "3.2.2. Form Quên mật khẩu & Đặt lại mật khẩu (FrmForgotPassword & FrmResetPassword)")
    add_p(doc, 
        "Giao diện khôi phục mật khẩu gồm 2 bước: Bước 1 yêu cầu nhập Email để nhận mã OTP 6 số qua Gmail kèm bộ đếm ngược "
        "60 giây; Bước 2 cho phép nhập mã OTP xác thực và mở form đặt lại mật khẩu mới.")
    add_ui_placeholder(doc, "Giao diện gửi mã OTP và xác thực khôi phục mật khẩu qua Email", "2", "Form Quên Mật Khẩu qua mã OTP Email SMTP")

    add_h3(doc, "3.2.3. Form Chính Dashboard Tổng quan (Form1 & TrangChuView)")
    add_p(doc, 
        "Màn hình trung tâm của phần mềm với Sidebar điều hướng bên trái và vùng hiển thị dữ liệu bên phải: Thanh điều hướng "
        "chọn Tháng/Năm, 3 Thẻ thống kê tài chính (Tổng Thu, Tổng Chi, Còn Lại), Bảng giao dịch gần đây và Cột 'Nhìn Lại' phân tích danh mục.")
    add_ui_placeholder(doc, "Màn hình Tổng quan Dashboard với 3 thẻ tài chính và cột Nhìn Lại", "3", "Form Tổng Quan Dashboard theo dõi Thu - Chi - Số dư")

    add_h3(doc, "3.2.4. Form Nhập giao dịch Thu/Chi (NhapVaoView)")
    add_p(doc, 
        "Giao diện cho phép người dùng chọn Loại giao dịch (Chi Tiêu / Thu Nhập), Danh mục tương ứng, nhập Số tiền (tự động phân cách "
        "hàng nghìn), Ngày phát sinh và Ghi chú; tích hợp kiểm tra cảnh báo ngân sách tự động.")
    add_ui_placeholder(doc, "Giao diện nhập mới khoản thu nhập hoặc chi tiêu", "4", "Form Nhập mới giao dịch dòng tiền")

    add_h3(doc, "3.2.5. Form Lịch sử biến động số dư (LichView)")
    add_p(doc, 
        "Màn hình quản lý toàn bộ giao dịch đã ghi nhận với thanh tìm kiếm từ khóa, bộ lọc Loại (Tất cả / Thu Nhập / Chi Tiêu), "
        "bảng hiển thị thứ tự cột [Loại | Danh Mục | Thời Gian] tô màu nổi bật, và các nút Cập nhật, Xóa giao dịch.")
    add_ui_placeholder(doc, "Giao diện danh sách lịch sử giao dịch và biến động số dư", "5", "Form Lịch sử giao dịch và biến động số dư")

    add_h3(doc, "3.2.6. Form Báo cáo & Thống kê biểu đồ (ThongKeView)")
    add_p(doc, 
        "Giao diện cung cấp công cụ trực quan hóa dữ liệu với bộ lọc Tháng, Năm và ComboBox chọn 3 chế độ biểu đồ: "
        "Biểu đồ cột theo ngày, Biểu đồ cột 12 tháng và Biểu đồ tròn tỷ trọng chi tiêu.")
    add_ui_placeholder(doc, "Giao diện hiển thị các dạng biểu đồ phân tích tài chính", "6", "Form Báo cáo Thống kê tài chính bằng biểu đồ Chart")

    add_h3(doc, "3.2.7. Form Quản lý Danh mục (CategoriesView)")
    add_p(doc, 
        "Giao diện quản lý danh mục phân loại gồm danh sách DataGridView hiển thị danh mục theo loại, ô nhập tên danh mục, "
        "ComboBox chọn loại và các nút Thêm, Sửa, Xóa.")
    add_ui_placeholder(doc, "Giao diện thêm, sửa, xóa các danh mục thu chi", "7", "Form Quản lý danh mục thu chi cá nhân")

    add_h3(doc, "3.2.8. Form Thiết lập Ngân sách (BudgetView)")
    add_p(doc, 
        "Giao diện cho phép đặt hạn mức chi tiêu tối đa cho từng danh mục trong tháng; bảng danh sách hiển thị số tiền hạn mức, "
        "số tiền đã tiêu, số tiền còn lại và thanh tiến độ ProgressBar có màu cảnh báo trực quan.")
    add_ui_placeholder(doc, "Giao diện thiết lập hạn mức ngân sách và thanh tiến độ cảnh báo", "8", "Form Thiết lập ngân sách chi tiêu và cảnh báo vượt mức")

    add_h3(doc, "3.2.9. Form Quản lý Sổ nợ & Khoản vay (DebtView)")
    add_p(doc, 
        "Giao diện quản lý công nợ với 2 tab hoặc bộ lọc Tôi Nợ / Cho Vay; hiển thị danh sách khoản nợ, số tiền, ngày hẹn trả, "
        "trạng thái và nút 'Thanh toán nợ' tự động đồng bộ vào dòng tiền.")
    add_ui_placeholder(doc, "Giao diện quản lý các khoản vay nợ và thanh toán nợ", "9", "Form Quản lý khoản vay và sổ nợ cá nhân")

    add_h3(doc, "3.2.10. Form Hồ sơ cá nhân & Đổi mật khẩu (UserProfileForm)")
    add_p(doc, 
        "Giao diện hiển thị thông tin tài khoản người dùng, cho phép cập nhật Họ tên, Giới tính, Ngày sinh, Địa chỉ (Email ở chế độ "
        "ReadOnly để đảm bảo toàn vẹn định danh) và tính năng đổi mật khẩu hiện tại.")
    add_ui_placeholder(doc, "Giao diện cập nhật hồ sơ cá nhân và đổi mật khẩu tài khoản", "10", "Form Hồ sơ thông tin cá nhân và đổi mật khẩu")

    # =========================================================================
    # CHƯƠNG 4: CÀI ĐẶT CHƯƠNG TRÌNH
    # =========================================================================
    add_h1(doc, "4. Cài đặt chương trình")

    add_h2(doc, "4.1. Môi trường phát triển")
    add_p(doc, "Hệ thống được phát triển dựa trên nền tảng công nghệ tiên tiến của Microsoft và cộng đồng mã nguồn mở:")
    add_bullet(doc, "C# phiên bản 12 trên nền tảng Windows Forms, cho phép xây dựng ứng dụng máy tính trực quan, khả năng xử lý sự kiện mượt mà và tận dụng kho điều khiển phong phú của Windows.", "Ngôn ngữ lập trình: ")
    add_bullet(doc, "Microsoft .NET 10 (Long-Term Support), phiên bản mới nhất mang lại hiệu năng biên dịch vượt trội, quản lý bộ nhớ tự động tối ưu và khả năng tương thích cao.", "Nền tảng Framework: ")
    add_bullet(doc, "Visual Studio 2022 Professional / Community, cung cấp bộ công cụ gỡ lỗi (debugger) mạnh mẽ, trình thiết kế đồ họa kéo thả (Windows Forms Designer) và quản lý gói thư viện NuGet thuận tiện.", "Môi trường phát triển tích hợp (IDE): ")
    add_bullet(doc, "PostgreSQL phiên bản 16, đảm bảo tính toàn vẹn CSDL, hỗ trợ các hàm xử lý chuỗi ngày tháng nâng cao như DATE_TRUNC và tốc độ truy vấn cao.", "Hệ quản trị cơ sở dữ liệu: ")
    add_bullet(doc, "Npgsql (phiên bản 10.0) - ADO.NET Data Provider cho PostgreSQL; System.Net.Mail - thư viện gửi email qua giao thức SMTP; System.Windows.Forms.DataVisualization - thư viện vẽ biểu đồ phân tích.", "Các thư viện mở rộng: ")

    add_h2(doc, "4.2. Cấu trúc thư mục dự án")
    add_p(doc, 
        "Phần mềm được tổ chức hoàn chỉnh theo kiến trúc 3 lớp (3-Tier Architecture), phân tách triệt để giữa tầng giao diện, "
        "tầng nghiệp vụ và tầng truy cập dữ liệu, giúp dự án đạt tính module hóa cao, dễ bảo trì, dễ kiểm thử và mở rộng:")
    add_bullet(doc, "Chứa các Form và UserControl phụ trách hiển thị dữ liệu và tiếp nhận thao tác từ người dùng (Form1.cs, AuthForm.cs, TrangChuView.cs, NhapVaoView.cs, LichView.cs, ThongKeView.cs, CategoriesView.cs, BudgetView.cs, DebtView.cs, UserProfileForm.cs, FrmForgotPassword.cs, FrmResetPassword.cs).", "Presentation Layer (GUI / Views): ")
    add_bullet(doc, "Chứa các lớp logic kiểm tra điều kiện, tính toán số liệu và quyết định nghiệp vụ (UserBLL.cs, TransactionBLL.cs, CategoryBLL.cs, BudgetBLL.cs, DebtBLL.cs, StatisticBLL.cs, DashboardBLL.cs).", "Business Logic Layer (BLL): ")
    add_bullet(doc, "Chứa các lớp giao tiếp trực tiếp với PostgreSQL qua các câu lệnh SQL an toàn (DbConnection.cs, UserDAL.cs, TransactionDAL.cs, CategoryDAL.cs, BudgetDAL.cs, DebtDAL.cs, StatisticDAL.cs, DashboardDAL.cs).", "Data Access Layer (DAL): ")
    add_bullet(doc, "Chứa các lớp đối tượng thực thể đóng gói dữ liệu truyền tải giữa các tầng (UserDTO.cs, TransactionDTO.cs, CategoryDTO.cs, BudgetDTO.cs, DebtDTO.cs).", "Data Transfer Object (DTO): ")
    add_bullet(doc, "Chứa các dịch vụ ngoại vi độc lập, tiêu biểu là EmailService.cs phụ trách cấu hình và gửi thư điện tử qua máy chủ SMTP Gmail.", "Services: ")

    add_ui_placeholder(doc, "Cấu trúc tổ chức thư mục dự án trong Visual Studio Solution Explorer", "11", "Cấu trúc thư mục dự án theo mô hình 3 lớp (3-Tier Architecture)")

    add_h2(doc, "4.3. Các chức năng chính")

    # 4.3.1
    add_h3(doc, "4.3.1. Quản lý Tài khoản & Xác thực người dùng (AuthForm)")
    add_p(doc, 
        "Module đảm nhiệm xác thực thông tin đăng nhập, đăng ký tài khoản mới và bảo vệ định danh người dùng. "
        "Khi người dùng đăng nhập, mật khẩu được kiểm tra và mã người dùng (CurrentUserId) được lưu vào biến tĩnh toàn cục "
        "để phân quyền dữ liệu cho toàn bộ phiên làm việc.")
    add_bullet(doc, "Kiểm tra rỗng, gọi BLL xác thực, khởi tạo phiên làm việc và chuyển đến Form1.", "Xử lý Đăng nhập: ")
    add_bullet(doc, "Kiểm tra định dạng email regex, mã hóa mật khẩu và chèn người dùng mới vào cơ sở dữ liệu.", "Xử lý Đăng ký: ")
    add_code_placeholder(doc, "AuthForm.cs", "185 đến 223", "Sự kiện BtnLogin_Click và BtnRegister_Click - Xử lý đăng nhập và đăng ký người dùng", "BtnLogin_Click, BtnRegister_Click")
    add_code_placeholder(doc, "BLL/UserBLL.cs", "25 đến 86", "Logic nghiệp vụ xác thực đăng nhập và đăng ký tài khoản", "Login, Register")

    # 4.3.2
    add_h3(doc, "4.3.2. Quên Mật Khẩu qua OTP Email (FrmForgotPassword & EmailService)")
    add_p(doc, 
        "Module cung cấp giải pháp khôi phục mật khẩu thông minh thông qua giao thức SMTP. Hệ thống tự động sinh mã OTP "
        "ngẫu nhiên gồm 6 chữ số (từ 100000 đến 999999), lưu tạm trong bộ nhớ đệm (Dictionary Cache) kèm thời hạn hết hạn 5 phút, "
        "đồng thời kết nối đến máy chủ Google SMTP để gửi email thông báo mã bảo mật tới người dùng.")
    add_code_placeholder(doc, "Views/FrmForgotPassword.cs", "193 đến 285", "Gửi mã OTP qua email, đếm ngược 60s và xử lý nút Xác minh OTP", "BtnSendOtp_Click, BtnVerifyOtp_Click")
    add_code_placeholder(doc, "Services/EmailService.cs", "31 đến 75", "Kết nối máy chủ SMTP Gmail (Port 587, SSL) và gửi email định dạng HTML", "SendOtpEmailAsync")
    add_code_placeholder(doc, "BLL/UserBLL.cs", "140 đến 245", "Sinh mã OTP, kiểm tra thời hạn hiệu lực và lưu mật khẩu mới", "GenerateAndSendOtp, VerifyOtp, ResetPassword")

    # 4.3.3
    add_h3(doc, "4.3.3. Quản lý Giao Dịch Thu/Chi & Lịch Sử (NhapVaoView & LichView)")
    add_p(doc, 
        "Cho phép nhập mới và tra cứu các khoản tiền thu nhập, chi tiêu. Đặc biệt, trước khi thêm một giao dịch chi tiêu mới, "
        "hệ thống sẽ tự động gọi phương thức CheckBudgetAlert trong tầng BLL để đối chiếu với hạn mức ngân sách tháng đã lập ra "
        "và hiển thị cảnh báo cho người dùng.")
    add_code_placeholder(doc, "Views/NhapVaoView.cs", "130 đến 175", "Nhập giao dịch thu/chi, kích hoạt kiểm tra cảnh báo ngân sách trước khi ghi nhận", "BtnOk_Click")
    add_code_placeholder(doc, "Views/LichView.cs", "93 đến 150", "Tải danh sách giao dịch, sắp xếp cột [Loại | Danh Mục | Thời Gian] và tô màu hiển thị", "LoadData")
    add_code_placeholder(doc, "Views/LichView.cs", "183 đến 214", "Cập nhật thông tin giao dịch và Xóa giao dịch khỏi cơ sở dữ liệu", "BtnUpdate_Click, BtnDelete_Click")

    # 4.3.4
    add_h3(doc, "4.3.4. Quản lý Danh Mục Thu/Chi (CategoriesView)")
    add_p(doc, 
        "Hỗ trợ người dùng chủ động cấu hình các nhóm mục chi tiêu (Ăn uống, Giải trí,...) hoặc nguồn thu (Lương, Thưởng,...) "
        "theo thói quen sinh hoạt cá nhân. Mỗi danh mục gắn liền với UserId của người dùng đó.")
    add_code_placeholder(doc, "Views/CategoriesView.cs", "89 đến 125", "Các thao tác Thêm danh mục mới, Cập nhật tên danh mục và Xóa danh mục", "BtnAdd_Click, BtnUpdate_Click, BtnDelete_Click")
    add_code_placeholder(doc, "BLL/CategoryBLL.cs", "17 đến 52", "Xử lý kiểm tra dữ liệu rỗng và gọi DAL thực thi CRUD bảng Categories", "AddCategory, UpdateCategory, DeleteCategory")

    # 4.3.5
    add_h3(doc, "4.3.5. Thiết Lập Ngân Sách & Thuật Toán Cảnh Báo Vượt Mức (BudgetView & BudgetBLL)")
    add_p(doc, 
        "Đây là tính năng tài chính thông minh cốt lõi của phần mềm. Người dùng thiết lập số tiền chi tối đa cho một danh mục "
        "trong tháng. Tầng BLL cung cấp hàm CheckBudgetAlert để tính toán: nếu tổng chi mới đạt từ 80% đến dưới 100% thì trả về "
        "cảnh báo vàng; nếu vượt quá 100% thì trả về cảnh báo đỏ bằng MessageBox yêu cầu người dùng cân nhắc trước khi đồng ý lưu.")
    add_code_placeholder(doc, "BLL/BudgetBLL.cs", "59 đến 120", "Thuật toán kiểm tra ngưỡng cảnh báo ngân sách 80% và 100%", "CheckBudgetAlert")
    add_code_placeholder(doc, "Views/BudgetView.cs", "269 đến 303", "Sự kiện định dạng ô DataGridView, tô màu thanh tiến độ ProgressBar ngân sách", "DgvBudgets_CellFormatting")
    add_code_placeholder(doc, "Views/BudgetView.cs", "325 đến 355", "Lưu hạn mức chi tiêu cho danh mục vào cơ sở dữ liệu", "BtnSave_Click")

    # 4.3.6
    add_h3(doc, "4.3.6. Quản Lý Khoản Vay & Sổ Nợ (DebtView & DebtBLL)")
    add_p(doc, 
        "Module quản lý danh sách các khoản tiền mình vay của người khác ('Tôi Nợ') hoặc cho bạn bè mượn ('Cho Vay'). "
        "Điểm đặc biệt của module là tính năng 'Thanh toán nợ' (SettleDebtWithTransaction): Khi người dùng bấm xác nhận trả nợ "
        "hoặc thu nợ, hệ thống không chỉ đổi trạng thái khoản nợ sang 'Đã Trả' mà còn tự động chèn một giao dịch Thu/Chi vào bảng "
        "Transactions để phản ánh chính xác số tiền ra vào ví cá nhân.")
    add_code_placeholder(doc, "DAL/DebtDAL.cs", "152 đến 220", "Phương thức tất toán nợ và tự động sinh giao dịch đối ứng vào CSDL", "SettleDebtWithTransaction")
    add_code_placeholder(doc, "Views/DebtView.cs", "344 đến 378", "Thêm khoản nợ/cho vay mới kèm số tiền, ngày hẹn trả và đối tác", "BtnAdd_Click")
    add_code_placeholder(doc, "Views/DebtView.cs", "429 đến 455", "Sự kiện bấm nút Thanh toán/Thu hồi nợ và kích hoạt cập nhật dòng tiền", "BtnSettle_Click")

    # 4.3.7
    add_h3(doc, "4.3.7. Thống Kê & Vẽ Biểu Đồ Trực Quan (ThongKeView & StatisticBLL)")
    add_p(doc, 
        "Hỗ trợ 3 chế độ hiển thị biểu đồ tài chính bằng WinForms Chart. Module xử lý làm sạch dữ liệu, ẩn các nhãn số tiền bằng 0 "
        "và vô hiệu hóa SmartLabel callout lines để biểu đồ luôn sáng sủa, không bị các vệt đen rối mắt.")
    add_code_placeholder(doc, "Views/ThongKeView.cs", "200 đến 315", "Khởi tạo Series và vẽ biểu đồ cột so sánh thu chi các ngày trong tháng (Chế độ 0)", "LoadData (Mode 0)")
    add_code_placeholder(doc, "Views/ThongKeView.cs", "316 đến 368", "Vẽ biểu đồ cột so sánh thu chi tổng thể của 12 tháng trong năm (Chế độ 1)", "LoadData (Mode 1)")
    add_code_placeholder(doc, "Views/ThongKeView.cs", "369 đến 430", "Vẽ biểu đồ tròn (Pie Chart) thể hiện tỷ trọng cơ cấu chi tiêu danh mục (Chế độ 2)", "LoadData (Mode 2)")

    # 4.3.8
    add_h3(doc, "4.3.8. Trang Chủ / Dashboard Tổng Quan (TrangChuView & DashboardBLL)")
    add_p(doc, 
        "Cung cấp cụm điều hướng chọn Tháng/Năm linh hoạt ([◀] Tháng MM/yyyy [▶] [Tháng Này]), tự động tải lại thẻ Tổng Thu, "
        "Tổng Chi, Còn Lại; tải bảng giao dịch gần đây và cột 'Nhìn Lại' phân tích cơ cấu các khoản thu chi trong tháng "
        "(Thu Nhập hiển thị màu xanh lá, Chi Tiêu hiển thị màu đỏ).")
    add_code_placeholder(doc, "Views/TrangChuView.cs", "229 đến 252", "Hàm ChangeMonth và UpdateMonthDisplay điều hướng chuyển tháng linh hoạt", "ChangeMonth, UpdateMonthDisplay")
    add_code_placeholder(doc, "Views/TrangChuView.cs", "254 đến 390", "Phương thức nạp dữ liệu Dashboard tổng hợp và phân tích danh mục cột Nhìn Lại", "LoadData")

    # =========================================================================
    # CHƯƠNG 5: KẾT QUẢ THỰC NGHIỆM
    # =========================================================================
    add_h1(doc, "5. Kết quả thực nghiệm")

    add_h2(doc, "5.1. Giao diện chính")
    add_p(doc, 
        "Ứng dụng sau khi hoàn thiện sở hữu giao diện chuyên nghiệp, trực quan với bố cục Sidebar bên trái hỗ trợ chuyển đổi mượt mà "
        "giữa các màn hình chức năng. Dưới đây là các hình ảnh ghi nhận quá trình điều hướng giao diện chính:")

    add_ui_placeholder(doc, "Giao diện chính đang điều hướng màn hình Tổng Quan (Dashboard)", "5.1.1", "Giao diện chính hiển thị màn hình Tổng Quan (Dashboard)")
    add_ui_placeholder(doc, "Giao diện chính đang điều hướng chức năng Nhập giao dịch thu/chi", "5.1.2", "Giao diện chính điều hướng màn hình Nhập Khoản Thu/Chi")
    add_ui_placeholder(doc, "Giao diện chính đang điều hướng chức năng Lịch sử biến động số dư", "5.1.3", "Giao diện chính điều hướng màn hình Lịch Sử Giao Dịch")
    add_ui_placeholder(doc, "Giao diện chính đang điều hướng chức năng Báo cáo Thống kê biểu đồ", "5.1.4", "Giao diện chính điều hướng màn hình Báo Cáo Thống Kê")
    add_ui_placeholder(doc, "Giao diện chính đang điều hướng chức năng Quản lý Danh mục", "5.1.5", "Giao diện chính điều hướng màn hình Quản Lý Danh Mục")
    add_ui_placeholder(doc, "Giao diện chính đang điều hướng chức năng Thiết lập Ngân sách", "5.1.6", "Giao diện chính điều hướng màn hình Thiết Lập Ngân Sách")
    add_ui_placeholder(doc, "Giao diện chính đang điều hướng chức năng Sổ Vay Nợ", "5.1.7", "Giao diện chính điều hướng màn hình Sổ Khoản Vay & Nợ")
    add_ui_placeholder(doc, "Giao diện chính đang điều hướng màn hình Thông tin Hồ sơ cá nhân", "5.1.8", "Giao diện chính điều hướng màn hình Thông Tin Tài Khoản")

    add_h2(doc, "5.2. Các chức năng hoạt động thực tế")
    add_p(doc, 
        "Dưới đây là mô tả chi tiết các bước vận hành thực nghiệm từng chức năng nghiệp vụ trọng yếu của phần mềm:")

    # 1. Đăng ký & Đăng nhập
    add_p(doc, "Chức năng Đăng nhập & Đăng ký tài khoản:", bold_prefix="1. ", indent=False)
    add_bullet(doc, "Tại màn hình khởi động, người dùng chọn tab 'Tạo tài khoản' và điền đầy đủ Họ tên, Email, Mật khẩu.")
    add_bullet(doc, "Bấm nút 'Tạo tài khoản': hệ thống kiểm tra tính hợp lệ và thông báo đăng ký thành công.")
    add_bullet(doc, "Chuyển sang tab 'Đăng nhập', nhập Email và Mật khẩu vừa tạo, bấm 'Đăng nhập' để vào màn hình chính.")
    add_ui_placeholder(doc, "Minh chứng thao tác Đăng ký và Đăng nhập thành công vào hệ thống", "5.2.1", "Thao tác Đăng ký và Đăng nhập hệ thống thành công")

    # 2. Quên mật khẩu qua OTP Email
    add_p(doc, "Chức năng Quên mật khẩu và Khôi phục qua Gmail OTP:", bold_prefix="2. ", indent=False)
    add_bullet(doc, "Tại giao diện Đăng nhập, người dùng bấm vào liên kết 'Quên mật khẩu?'.")
    add_bullet(doc, "Form FrmForgotPassword xuất hiện, người dùng nhập Email và nhấn 'Gửi mã OTP'.")
    add_bullet(doc, "Kiểm tra hộp thư Gmail, nhận thư chứa mã OTP 6 số do hệ thống gửi tự động.")
    add_bullet(doc, "Nhập mã OTP vào ô xác minh và bấm 'Xác minh OTP'. Hệ thống mở form FrmResetPassword cho phép nhập mật khẩu mới và lưu thay đổi.")
    add_ui_placeholder(doc, "Minh chứng nhận mã OTP qua Gmail và màn hình xác minh khôi phục mật khẩu", "5.2.2", "Quy trình xác thực mã OTP gửi về Email để khôi phục mật khẩu")

    # 3. Thêm giao dịch & Cảnh báo ngân sách
    add_p(doc, "Chức năng Nhập giao dịch & Cảnh báo hạn mức ngân sách:", bold_prefix="3. ", indent=False)
    add_bullet(doc, "Người dùng mở tab 'Nhập Vào', chọn loại 'Chi Tiêu', chọn danh mục 'Ăn uống', nhập số tiền 1,500,000 đ.")
    add_bullet(doc, "Bấm nút 'LƯU LẠI': Hệ thống lập tức phát hiện tổng chi của danh mục 'Ăn uống' vượt quá hạn mức ngân sách tháng đã đặt ra.")
    add_bullet(doc, "Hộp thoại MessageBox cảnh báo đỏ xuất hiện thông báo: 'Cảnh báo: Khoản chi này sẽ làm vượt quá 100% ngân sách danh mục! Bạn có chắc chắn muốn tiếp tục không?'.")
    add_bullet(doc, "Nếu người dùng bấm 'Yes', giao dịch được ghi nhận thành công và số dư được cập nhật.")
    add_ui_placeholder(doc, "Minh chứng hộp thoại cảnh báo ngân sách chi tiêu xuất hiện khi lưu giao dịch", "5.2.3", "Hộp thoại cảnh báo vượt ngưỡng hạn mức ngân sách chi tiêu")

    # 4. Thiết lập ngân sách
    add_p(doc, "Chức năng Thiết lập hạn mức ngân sách theo tháng:", bold_prefix="4. ", indent=False)
    add_bullet(doc, "Mở tab 'Ngân Sách', chọn Tháng/Năm và chọn Danh mục cần đặt hạn mức.")
    add_bullet(doc, "Nhập số tiền hạn mức (ví dụ: 3,000,000 đ) và bấm 'Lưu hạn mức'.")
    add_bullet(doc, "Bảng ngân sách hiển thị ngay lập tức danh mục kèm thanh tiến độ: tỷ lệ phần trăm đã tiêu và số tiền còn lại có thể chi trong tháng.")
    add_ui_placeholder(doc, "Minh chứng bảng theo dõi ngân sách với thanh tiến độ phân cấp màu sắc", "5.2.4", "Danh sách ngân sách với thanh tiến độ trực quan theo màu")

    # 5. Quản lý sổ nợ & Tất toán nợ
    add_p(doc, "Chức năng Quản lý khoản vay nợ và Thanh toán nợ:", bold_prefix="5. ", indent=False)
    add_bullet(doc, "Mở tab 'Sổ Vay Nợ', chọn 'Tôi Nợ' hoặc 'Cho Vay', nhập tên đối tác, số tiền và ngày hẹn trả.")
    add_bullet(doc, "Bấm 'Thêm mới' để ghi nhận khoản vay.")
    add_bullet(doc, "Khi trả hết nợ hoặc nhận lại tiền, chọn bản ghi trong bảng và nhấn nút 'Thanh toán nợ'.")
    add_bullet(doc, "Hệ thống đổi trạng thái nợ sang 'Đã Trả' và tự động tạo giao dịch đối ứng vào lịch sử dòng tiền.")
    add_ui_placeholder(doc, "Minh chứng thao tác thanh toán nợ và tự động cập nhật vào lịch sử giao dịch", "5.2.5", "Thao tác thanh toán nợ và tự động ghi nhận vào dòng tiền")

    # 6. Quản lý danh mục & Lịch sử
    add_p(doc, "Chức năng Quản lý danh mục và Tra cứu lịch sử dòng tiền:", bold_prefix="6. ", indent=False)
    add_bullet(doc, "Thêm các danh mục mới hoặc chỉnh sửa tên danh mục tùy thích.")
    add_bullet(doc, "Tại màn hình 'Lịch', người dùng có thể tra cứu giao dịch theo từ khóa, lọc xem riêng 'Thu Nhập' hoặc 'Chi Tiêu', sửa số tiền hoặc xóa giao dịch sai lệch.")
    add_ui_placeholder(doc, "Minh chứng bảng lịch sử giao dịch có phân loại màu sắc và chức năng tìm kiếm", "5.2.6", "Màn hình lịch sử biến động số dư với bộ lọc tìm kiếm linh hoạt")

    add_h2(doc, "5.3. Kết quả báo cáo / thống kê")
    add_p(doc, 
        "Hệ thống cung cấp bức tranh tài chính toàn diện qua 3 chế độ biểu đồ hiện đại:")

    add_p(doc, "1. Biểu đồ cột so sánh Thu Nhập - Chi Tiêu các ngày trong tháng:", bold_prefix="", indent=False)
    add_p(doc, 
        "Hiển thị từng ngày trong tháng trên trục hoành; mỗi ngày có 2 cột màu: Cột xanh lá biểu thị Thu Nhập và Cột đỏ biểu thị "
        "Chi Tiêu. Các ngày có phát sinh giao dịch hiển thị rõ số tiền trên đầu cột, những ngày không phát sinh được xử lý thông minh "
        "không vẽ nhãn rối mắt.")
    add_ui_placeholder(doc, "Minh chứng biểu đồ cột so sánh thu chi theo từng ngày trong tháng", "5.3.1", "Biểu đồ cột so sánh thu chi từng ngày trong tháng")

    add_p(doc, "2. Biểu đồ cột tổng quan thu chi 12 tháng trong năm:", bold_prefix="", indent=False)
    add_p(doc, 
        "Tổng hợp số liệu của cả 12 tháng trong năm được chọn, giúp người dùng so sánh được tháng nào có thu nhập cao nhất "
        "hoặc tháng nào bị thâm hụt tài chính để điều chỉnh ngân sách cho năm sau.")
    add_ui_placeholder(doc, "Minh chứng biểu đồ cột tổng hợp thu chi 12 tháng trong năm", "5.3.2", "Biểu đồ so sánh thu chi các tháng trong năm")

    add_p(doc, "3. Biểu đồ tròn (Pie Chart) phân tích cơ cấu chi tiêu:", bold_prefix="", indent=False)
    add_p(doc, 
        "Trực quan hóa tỷ trọng phần trăm chi tiêu của từng danh mục trong tháng (ví dụ: Ăn uống chiếm 45%, Nhà ở 25%, Mua sắm 15%,...). "
        "Mỗi danh mục mang một lát cắt màu sắc riêng biệt kèm tỷ lệ phần trăm rõ ràng.")
    add_ui_placeholder(doc, "Minh chứng biểu đồ tròn phân tích cơ cấu tỷ trọng chi tiêu", "5.3.3", "Biểu đồ tròn cơ cấu tỷ trọng chi tiêu theo danh mục")

    # =========================================================================
    # CHƯƠNG 6: ĐÁNH GIÁ – HẠN CHẾ – HƯỚNG PHÁT TRIỂN
    # =========================================================================
    add_h1(doc, "6. Đánh giá – Hạn chế – Hướng phát triển")

    add_h2(doc, "6.1. Đánh giá")
    add_bullet(doc, "Phần mềm đáp ứng đầy đủ và vượt trội các yêu cầu đặt ra ban đầu về một hệ thống quản lý tài chính cá nhân hoàn chỉnh, xử lý chính xác các bài toán thu chi, ngân sách, vay nợ và thống kê.", "Mức độ hoàn thiện: ")
    add_bullet(doc, "Mô hình kiến trúc 3 lớp (3-Tier) được áp dụng bài bản và chặt chẽ, tách biệt rõ ràng giữa Presentation, BLL và DAL, giúp mã nguồn sáng sủa, dễ đọc, dễ mở rộng và kiểm thử.", "Chất lượng mã nguồn: ")
    add_bullet(doc, "Cơ sở dữ liệu PostgreSQL đảm bảo tính toàn vẹn quan hệ (ACID), truy vấn nhanh chóng và xử lý nhóm dữ liệu thời gian chính xác.", "Độ tin cậy & Hiệu năng: ")
    add_bullet(doc, "Giao diện hiện đại, thân thiện, phối màu hài hòa, có các điểm nhấn thông minh như cảnh báo ngân sách tự động, thanh tiến độ màu sắc và gửi OTP qua Gmail SMTP.", "Trải nghiệm người dùng: ")

    add_h2(doc, "6.2. Hạn chế")
    add_bullet(doc, "Ứng dụng hiện chỉ hoạt động trên hệ điều hành Windows dưới dạng Desktop WinForms, chưa có phiên bản chạy trên trình duyệt Web hoặc điện thoại di động (Android / iOS).", "Nền tảng triển khai: ")
    add_bullet(doc, "Dữ liệu thu chi phụ thuộc hoàn toàn vào việc người dùng tự tay ghi nhận; hệ thống chưa kết nối trực tiếp với API của các ngân hàng thương mại để tự động đồng bộ giao dịch quẹt thẻ.", "Tự động hóa dữ liệu: ")
    add_bullet(doc, "Chưa hỗ trợ xuất báo cáo tài chính hàng tháng/hàng năm ra các định dạng văn phòng phổ biến như Excel (.xlsx) hay PDF.", "Xuất báo cáo ngoài: ")

    add_h2(doc, "6.3. Hướng phát triển")
    add_bullet(doc, "Mở rộng hệ thống sang phiên bản ứng dụng di động đa nền tảng bằng .NET MAUI hoặc Flutter, giúp người dùng ghi chép chi tiêu mọi lúc mọi nơi ngay khi vừa thanh toán.", "Phát triển phiên bản Mobile: ")
    add_bullet(doc, "Tích hợp thư viện EPPlus và iTextSharp để cho phép người dùng xuất báo cáo tài chính chi tiết ra bảng tính Excel và hồ sơ PDF chuyên nghiệp.", "Xuất báo cáo Excel / PDF: ")
    add_bullet(doc, "Triển khai cơ sở dữ liệu PostgreSQL lên môi trường điện toán đám mây (Cloud Database như AWS RDS, Supabase hoặc Microsoft Azure Database) để hỗ trợ đồng bộ dữ liệu thời gian thực giữa máy tính và điện thoại.", "Đồng bộ hóa Cloud: ")
    add_bullet(doc, "Tích hợp mô hình học máy (Machine Learning / AI) để phân tích lịch sử thói quen tiêu dùng, dự báo dòng tiền tháng tiếp theo và đưa ra các gợi ý tiết kiệm tài chính cá nhân thông minh.", "Tích hợp Trí tuệ nhân tạo (AI): ")

    # =========================================================================
    # CHƯƠNG 7: KẾT LUẬN
    # =========================================================================
    add_h1(doc, "7. Kết luận")

    add_p(doc, "Tóm tắt mục tiêu đã thực hiện:", bold_prefix="• ", indent=False)
    add_p(doc, 
        "Trong khuôn khổ học phần 'Lập trình .NET 1 + BTL', tác giả đã hoàn thành mục tiêu xây dựng một ứng dụng Windows Forms "
        "hoàn chỉnh phục vụ công tác Quản lý chi tiêu cá nhân (Personal Expense Tracker). Ứng dụng đã giải quyết trọn vẹn bài toán ghi nhận "
        "dòng tiền, phân loại danh mục, thiết lập hạn mức ngân sách thông minh kèm cảnh báo tự động, theo dõi sổ nợ cá nhân, trực quan hóa "
        "báo cáo tài chính bằng biểu đồ và bảo mật tài khoản bằng mã OTP gửi qua Gmail SMTP.")

    add_p(doc, "Kết quả đạt được:", bold_prefix="• ", indent=False)
    add_bullet(doc, "Hoàn thiện 100% các chức năng cốt lõi và các tính năng nâng cao theo đúng thiết kế.")
    add_bullet(doc, "Phần mềm chạy ổn định, không phát sinh lỗi ngoại lệ, giao diện hiển thị mượt mà trên Windows.")
    add_bullet(doc, "Cơ sở dữ liệu PostgreSQL hoạt động chính xác, đảm bảo ràng buộc toàn vẹn khóa ngoại và tối ưu hóa truy vấn.")
    add_bullet(doc, "Báo cáo thống kê biểu đồ trực quan, hỗ trợ hiệu quả cho việc theo dõi và đánh giá sức khỏe tài chính cá nhân.")

    add_p(doc, "Cảm nhận và bài học kinh nghiệm:", bold_prefix="• ", indent=False)
    add_bullet(doc, "Nắm vững quy trình phát triển một phần mềm hoàn chỉnh từ khâu khảo sát bài toán, phân tích yêu cầu, thiết kế cơ sở dữ liệu, thiết kế giao diện UI/UX đến cài đặt và kiểm thử.", "Hiểu sâu quy trình công nghệ phần mềm: ")
    add_bullet(doc, "Thành thạo ngôn ngữ lập trình C# hiện đại, công nghệ WinForms, kết nối và tương tác với PostgreSQL thông qua ADO.NET (Npgsql), áp dụng mô hình 3 lớp (3-Tier Architecture) vào thực tế.", "Nâng cao kỹ năng lập trình & CSDL: ")
    add_bullet(doc, "Học được cách tích hợp dịch vụ mạng thực tế như giao thức SMTP gửi email OTP qua Google Server và điều khiển thư viện đồ họa vẽ biểu đồ Chart phức tạp.", "Làm chủ công nghệ và dịch vụ bên ngoài: ")
    add_bullet(doc, "Nhận thức rõ ràng rằng một phần mềm tốt không chỉ cần chạy đúng nghiệp vụ mà còn phải có giao diện thân thiện, xử lý ngoại lệ chu đáo và mang lại giá trị thiết thực cho người dùng cuối.", "Tư duy sản phẩm người dùng: ")

    # =========================================================================
    # CHƯƠNG 8: TÀI LIỆU THAM KHẢO
    # =========================================================================
    add_h1(doc, "8. Tài liệu tham khảo")
    
    add_p(doc, "[1] ThS. Ngô Hùng Long (2025 - 2026), Bài giảng Lập trình .NET 1 và hướng dẫn thực hành Bài tập lớn, Trường Đại học Mỏ - Địa chất (HUMG).", indent=False)
    add_p(doc, "[2] Microsoft Learn (2025), Windows Forms Documentation for .NET, truy cập tại: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/", indent=False)
    add_p(doc, "[3] Microsoft Learn (2025), Common Web Application Architectures & 3-Tier Architecture in C#, truy cập tại: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures", indent=False)
    add_p(doc, "[4] PostgreSQL Global Development Group (2025), PostgreSQL 16 Documentation, truy cập tại: https://www.postgresql.org/docs/16/", indent=False)
    add_p(doc, "[5] The Npgsql Development Team (2025), Npgsql - .NET Data Provider for PostgreSQL Documentation, truy cập tại: https://www.npgsql.org/doc/", indent=False)
    add_p(doc, "[6] Microsoft Learn (2025), System.Net.Mail Namespace & SmtpClient Class, truy cập tại: https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient", indent=False)
    add_p(doc, "[7] Microsoft Learn (2025), System.Windows.Forms.DataVisualization.Charting Namespace, truy cập tại: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.datavisualization.charting", indent=False)

    print(f"Saving final report to {output_path}...")
    doc.save(output_path)
    print("Report generated successfully!")

if __name__ == "__main__":
    generate_btl_report()
