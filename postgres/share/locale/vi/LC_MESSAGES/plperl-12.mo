��    *      l  ;   �      �     �  #   �     �  I   �     5     R  7   h  '   �  '   �  *   �  C     D   _  K   �  W   �  ;   H  .   �  1   �      �  .     $   5     Z  8   n  &   �  .   �  J   �  '   H     p  L   �  @   �  5   	  E   N	  P   �	  >   �	  0   $
     U
  4   q
  %   �
  &   �
     �
  !     !   -  �  O     �  @        B  �   E     �     �  O     1   Q  .   �  9   �  P   �  Q   =  Q   �  i   �  X   K  N   �  Z   �  6   N  C   �     �     �  D      7   E  ?   }  g   �  5   %  (   [  S   �  [   �  U   4  O   �  m   �  l   H  E   �     �  9     *   R  (   }     �  1   �  ,   �                                       &   "          	                                         
       (   $                                 %             !           #   '   *                         )       $_TD->{new} does not exist $_TD->{new} is not a hash reference %s If true, trusted and untrusted Perl code will be compiled in strict mode. PL/Perl anonymous code block PL/Perl function "%s" PL/Perl function must return reference to hash or array PL/Perl functions cannot accept type %s PL/Perl functions cannot return type %s Perl hash contains nonexistent column "%s" Perl initialization code to execute once when plperl is first used. Perl initialization code to execute once when plperlu is first used. Perl initialization code to execute when a Perl interpreter is initialized. SETOF-composite-returning PL/Perl function must call return_next with reference to hash cannot allocate multiple Perl interpreters on this platform cannot convert Perl array to non-array type %s cannot convert Perl hash to non-composite type %s cannot set system attribute "%s" cannot use return_next in a non-SETOF function compilation of PL/Perl function "%s" couldn't fetch $_TD didn't get a CODE reference from compiling function "%s" didn't get a return item from function didn't get a return item from trigger function function returning record called in context that cannot accept type record ignoring modified row in DELETE trigger lookup failed for type %s multidimensional arrays must have array expressions with matching dimensions number of array dimensions (%d) exceeds the maximum allowed (%d) query result has too many rows to fit in a Perl array result of PL/Perl trigger function must be undef, "SKIP", or "MODIFY" set-returning PL/Perl function must return reference to array or use return_next set-valued function called in context that cannot accept a set trigger functions can only be called as triggers while executing PLC_TRUSTED while executing PostgreSQL::InServer::SPI::bootstrap while executing plperl.on_plperl_init while executing plperl.on_plperlu_init while executing utf8fix while parsing Perl initialization while running Perl initialization Project-Id-Version: plperl (PostgreSQL) 11
Report-Msgid-Bugs-To: pgsql-bugs@postgresql.org
PO-Revision-Date: 2018-04-29 23:57+0900
Language-Team: <pgvn_translators@postgresql.vn>
MIME-Version: 1.0
Content-Type: text/plain; charset=UTF-8
Content-Transfer-Encoding: 8bit
X-Generator: Poedit 2.0.6
Last-Translator: Dang Minh Huong <kakalot49@gmail.com>
Plural-Forms: nplurals=1; plural=0;
Language: vi_VN
 $_TD->{new} không tồn tại $_TD->{new} không phải là một tham chiếu giá trị băm %s Nếu đúng, mã perl đáng tin cậy(PL/Perl ) và không đáng tin cậy(PL/PerlU) sẽ được biên dịch trong chế độ strict. Khối mã ẩn danh PL/Perl Hàm PL/Perl "%s" Hàm PL/Perl phải trả về tham thiếu tới giá trị băm hoặc mảng Hàm PL/Perl không thể chấp nhận kiểu %s Hàm PL/Perl không thể trả về kiểu %s Giá trị băm Perl chứa cột không tồn tại "%s" Mã Perl được thực thi khi plperl được sử dụng lần đầu tiên. Mã Perl được thực thi khi plperlu được sử dụng lần đầu tiên. Mã Perl được thực thi khi trình thông dịch Perl được khởi tạo. Hàm PL/Perl trả về SETOF-composite phải gọi return_next với tham chiếu tới giá trị băm không thể cấp phát nhiều trình thông dịch Perl trên hệ điều hành này không thể chuyển đổi mảng Perl thành kiểu không phải mảng %s không thể chuyển đổi giá trị băm Perl thành kiểu không phải-composite %s không thể thiết lập attribute hệ thống "%s" không thể sử dụng return_next trong hàm không phải-SETOF biên dịch hàm PL/Perl "%s" không thể fetch $_TD không nhận được tham chiếu CODE từ hàm biên dịch "%s" không nhận được một mục trả về từ hàm không nhận được một mục trả về từ hàm trigger hàm trả về bản ghi được gọi trong ngữ cảnh không thể chấp nhận kiểu bản ghi bỏ qua hàng đã sửa đổi trong trigger DELETE không tìm thấy kiểu dữ liệu %s mảng đa chiều phải có biểu thức mảng tương ứng với các chiều số lượng chiều của mảng (%d) vượt quá số lượng tối đa cho phép (%d) kết quả truy vấn có quá nhiều hàng có thể vừa với một mảng Perl kết quả của hàm trigger PL/Perl phải là undef, "SKIP" hoặc "MODIFY" hàm thiết lập-trả về PL/Perl phải trả về tham chiếu tới mảng hay sử dụng return_next hàm thiết lập giá trị được gọi trong ngữ cảnh không thể chấp nhận một tập hợp các hàm trigger chỉ có thể được gọi như những trigger trong khi chạy PLC_TRUSTED trong khi thực thi PostgreSQL::InServer::SPI::bootstrap trong khi thực thi plperl.on_plperl_init trong khi thực thi plperl.plperlu_init trong khi thực thi utf8fix trong khi phân tích cú pháp khởi tạo Perl trong khi chạy cú pháp khởi tạo Perl 