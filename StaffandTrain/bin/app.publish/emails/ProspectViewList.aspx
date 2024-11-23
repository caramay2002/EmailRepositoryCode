<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProspectViewList.aspx.cs" Inherits="StaffandTrain.ProspectViewList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">

    .fontprospect{
        font-size:16px;
    }

</style>
<link href="~/Content/loading.css" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server">

        <div>
            <div class="container main-ctn">
        <h1 class="title">Prospect View List(Recruiter)</h1>
    <div class="fontprospect" id="divlistid" data-listid="@ViewBag.listid"> <b>List Name : @ViewBag.listname</b></div>
    <div id="divmsg" class="alert alert-success" style="display:none"></div>

        <div id="" class="alert alert-success">@ViewBag.Message</div>
 
        <div class="">
            <div class="col_exp">
                <label></label>
                <div>
                    Return Path: <a href="/ProspectListsAdmin/Index?listid=@ViewBag.listid" class="">PROSPECT LISTS</a>
                </div>
                           
                
            </div>
            <div class="col_exp">
                <label></label>
                <a href="/ProspectViewList/exporttoexcel?listid=@ViewBag.listid" class="">Export Company List To Excel</a>
            </div>
            <div class="clearfix"></div>
            <div class="col_exp">
                <div>
                    <label></label>
                  <asp:DropDownList runat="server">
          </asp:DropDownList>
                    <a href="/ProspectViewList/exporttocontactexcel?listid=@ViewBag.listid" id="lnkconexport" class="btn lnk_export">Export Contact List To Excel</a>
                    <div id="" class="validateTips"></div>
                </div>
            </div>
        </div>
    <div class="clearfix"></div>
    <h1 class="title">Search</h1>
    <div class="admin-formNew">
        <div class="col-sm-4 form-group">
            <label>City Circle</label>
          <asp:DropDownList runat="server">
          </asp:DropDownList>
        
            <div id="" class="validateTips"></div>
        </div>
        <div class="col-sm-4 form-group">
            <label>Biztype</label>
            
            <asp:DropDownList runat="server">
          </asp:DropDownList>
            <div id="" class="validateTips"></div>
        </div>
        <div class="col-sm-4 form-group">
            <label>Title</label>
          
          <asp:DropDownList runat="server">
          </asp:DropDownList>
            <div id="" class="validateTips"></div>
        </div>
        <div class="col-sm-4 form-group">
            <label>Name </label>
         
             <asp:TextBox ID="txtname" runat="server" CssClass="form-control"></asp:TextBox>
            <div id="" class="validateTips"></div>
        </div>
        <div class="col-sm-4 form-group">
            <label>Notes</label>
        
            <asp:TextBox ID="txtnotes" runat="server" CssClass="form-control"></asp:TextBox>
            <div id="" class="validateTips"></div>
        </div>
        <div class="col-sm-4 form-group">
            <label></label>
            <div class="divcreate"><a href="javascript:void(0)" class="btn btn-success" id="lnksearch">Search </a> </div>
        </div>


     

    </div>

    <div class="divcreate">
        <a href="/ProspectViewList/SaveCompany" class="btn btn-success" id="lnkcreate">Create New   <i class="fa fa-plus" aria-hidden="true"></i></a>
     
            <a href="/ProspectFilterList/Index" class="btn btn-success" id="lnk">Click Here To Get Email List Filtered</a>
  
    </div>



    <div id="tblpros"></div>



</div>
        </div>
    </form>
</body>
</html>
