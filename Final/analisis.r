library(readxl)

rouge_conceptual <- read_excel("D:/Tesis2016/Jarvis/Lincoln/Rougue/rouge_final_conceptual.xlsx", sheet = "RST")
rouge_no_expantion_conceptual <- read_excel("D:/Tesis2016/Jarvis/Lincoln/Rougue/rouge_final_conceptual.xlsx", 
                                            sheet = "RSTNoExpantion")
#View(rouge_final_conceptual)
#View(rouge_conceptual)

rouge1_rst <- rouge_conceptual[,6]
rouge1_rst_noexp <- rouge_no_expantion_conceptual[,6]
rouge1_wilconxon = wilcox.test(rouge1_rst, rouge1_rst_noexp)
print(rouge1_wilconxon)
boxplot(rouge1_rst, rouge1_rst_noexp)






